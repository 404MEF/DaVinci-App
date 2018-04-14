using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using Newtonsoft.Json;
using Davinci.Api.Models;
using RestSharp.Authenticators;

namespace Davinci.Api
{
    public sealed class DavinciApi
    {
        const string BASE_URL = @"http://10.0.2.2:3000/api";

        private static string formatUri(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute) == false)
                return new UriBuilder(url).Uri.ToString();
            else
                return url;
        }

        private static T Get<T>(string resource, string url = BASE_URL) where T : BaseApiModel, new()
        {
            var client = new RestClient(formatUri(url));
            RestRequest request = new RestRequest(resource, Method.GET);

            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                T errorModel = new T();
                errorModel.result = "bad";
                errorModel.message = "Critical error";
                return errorModel;
            }
        }
        private static async Task<T> GetAsync<T>(string resource, string url = BASE_URL) where T : BaseApiModel,new()
        {
            var client = new RestClient(formatUri(url));
            RestRequest request = new RestRequest(resource, Method.GET);

            request.RequestFormat = DataFormat.Json;

            //var response = client.Execute(request);

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                T errorModel = new T();
                errorModel.result = "bad";
                errorModel.message = "Critical error";
                return errorModel;
            }
        }

        private static T Post<T>(string resource, string jsonBody = "", string url = BASE_URL, HttpBasicAuthenticator authenticator = null) where T : BaseApiModel, new()
        {
            var client = new RestClient(formatUri(url));
            client.Authenticator = authenticator;

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(jsonBody))
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = client.Execute(request);

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                T errorModel = new T();
                errorModel.result = "bad";
                errorModel.message = "Critical error";
                return errorModel;
            }
        }
        private static async Task<T> PostAsync<T>(string resource, string jsonBody = "", string url = BASE_URL, HttpBasicAuthenticator authenticator = null) where T : BaseApiModel, new()
        {
            var client = new RestClient(formatUri(url));
            client.Authenticator = authenticator;

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(jsonBody))
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch
            {
                T errorModel = new T();
                errorModel.result = "bad";
                errorModel.message = "Critical error";
                return errorModel;
            }
        }

        public static async Task<AuthenticationModel> Authenticate(string user, string password)
        {
            const string resource = @"account/authenticate";

            HttpBasicAuthenticator auth = new HttpBasicAuthenticator(user, password);

            return await PostAsync<AuthenticationModel>(resource, authenticator: auth);
        }

        public static async Task<BaseApiModel> Register(string user, string email, string password)
        {
            const string resource = @"account/register";

            string body = JsonConvert.SerializeObject(new { username = user, email = email, password = password });

            return await PostAsync<BaseApiModel>(resource, body);
        }

    }
}
