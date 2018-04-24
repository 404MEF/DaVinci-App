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
        private static RestClient _client;
        private static RestClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient(formatUri(BASE_URL));
                    _client.Timeout = 4000;
                }

                return _client;
            }
        }

        private static string formatUri(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute) == false)
                return new UriBuilder(url).Uri.ToString();
            else
                return url;
        }

        private static T Get<T>(string resource, string token = "") where T : BaseApiModel, new()
        {
            RestRequest request = new RestRequest(resource, Method.GET);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

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
        private static T Post<T>(string resource, string jsonBody = "", HttpBasicAuthenticator authenticator = null, string token = "") where T : BaseApiModel, new()
        {
            client.Authenticator = authenticator;

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

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

        private static async Task<T> GetAsync<T>(string resource, string token = "") where T : BaseApiModel, new()
        {
            RestRequest request = new RestRequest(resource, Method.GET);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

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
        private static async Task<T> PostAsync<T>(string resource, string jsonBody = "", HttpBasicAuthenticator authenticator = null, string token = "") where T : BaseApiModel, new()
        {
            client.Authenticator = authenticator;

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            //Send token with request
            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

            //Send body data
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

            var response = await PostAsync<AuthenticationModel>(resource, authenticator: auth);

            if (response.OK)
            {
                //Refresh Token
                Token.value = response.token;
            }

            return response;
        }

        public static async Task<BaseApiModel> Register(string user, string email, string password)
        {
            const string resource = @"account/register";

            string body = JsonConvert.SerializeObject(new { username = user, email = email, password = password });

            return await PostAsync<BaseApiModel>(resource, body);
        }

        public static async Task<BaseApiModel> VerifyToken(string token)
        {
            const string resource = @"account/test";

            return await PostAsync<BaseApiModel>(resource, token: token);
        }

        public static async Task<BaseApiModel> UploadPost(string data, string category)
        {
            const string resource = @"feed/";

            string body = JsonConvert.SerializeObject(new { image = data, category = category});

            return await PostAsync<BaseApiModel>(resource, body, token: Token.value);
        }

        public static async Task<PostCollectionModel> GetFeedPosts(int page)
        {
            const string resource = @"feed/";

            return await GetAsync<PostCollectionModel>(resource, token: Token.value);
        }

    }
}
