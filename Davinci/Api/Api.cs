using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using Newtonsoft.Json;
using Davinci.Api.Models;

namespace Davinci.Api
{
    public sealed class DavinciApi
    {
        const string BASE_URL = @"http://api/api";

        private static string formatUri(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute) == false)
                return new UriBuilder(url).Uri.ToString();
            else
                return url;
        }

        private static T Get<T>(string resource, string url = BASE_URL)
        {
            var client = new RestClient(formatUri(url));
            RestRequest request = new RestRequest(resource, Method.GET);

            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(response.Content);
            else
                return default(T);
        }
        private static async Task<T> GetAsync<T>(string resource, string url = BASE_URL)
        {
            var client = new RestClient(formatUri(url));
            RestRequest request = new RestRequest(resource, Method.GET);

            request.RequestFormat = DataFormat.Json;

            //var response = client.Execute(request);

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(response.Content);
            else
                return default(T);
        }

        private static T Post<T>(string resource, string jsonBody = "", string url = BASE_URL)
        {
            var client = new RestClient(formatUri(url));

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(jsonBody))
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.Continue)
                return JsonConvert.DeserializeObject<T>(response.Content);
            else
                return default(T);
        }
        private static async Task<T> PostAsync<T>(string resource, string jsonBody = "", string url = BASE_URL)
        {
            var client = new RestClient(formatUri(url));

            RestRequest request = new RestRequest(resource, Method.POST);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(jsonBody))
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.Continue)
                return JsonConvert.DeserializeObject<T>(response.Content);
            else
                return default(T);
        }

        public static async Task<AuthenticationModel> Authenticate(string user, string password)
        {
            const string resource = "authenticate";

            string body = JsonConvert.SerializeObject(new { username=user,password=password});

            return await PostAsync<AuthenticationModel>(resource, body);
        }

        //public static async Task<AccountConfigModel> GUID(string guid)
        //{
        //    string resource = "accountconfig/" + guid;

        //    return await GetAsync<AccountConfigModel>(resource);
        //}
        //public static async Task<UserProcessApprovalModel[]> UserProcessApprovals(string process, string platform)
        //{
        //    StringBuilder resource = new StringBuilder();

        //    resource.Append("approvals/").Append(ApplicationState.Login.user).Append('/').Append(platform).Append('/').Append(process);

        //    return await GetAsync<UserProcessApprovalModel[]>(resource.ToString(), ApplicationState.Api.API_PREFIX);
        //}
    }
}
