using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;
using Davinci.Api.Models;

namespace Davinci.Api
{
    public sealed class DavinciApi
    {
        //const string BASE_URL = @"http://10.0.2.2:3000/api";
        const string BASE_URL = @"http://davinci-env.qmicn3gpdm.eu-west-1.elasticbeanstalk.com/api";

        const int TIMEOUT = 5;

        private static RestClient _client;
        private static RestClient client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient(formatUri(BASE_URL));
                    _client.Timeout = 5 * 1000;
                    _client.ReadWriteTimeout = 5 * 1000;
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

        private static T Get<T>(string resource, string token = "", params KeyValuePair<string, string>[] parameters) where T : BaseApiModel, new()
        {
            RestRequest request = new RestRequest(resource, Method.GET);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

            if (parameters != null)
            {
                foreach (var keyValue in parameters)
                    request.AddQueryParameter(keyValue.Key, keyValue.Value);
            }

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

        private static async Task<T> GetAsync<T>(string resource, string token = "", params KeyValuePair<string, string>[] parameters) where T : BaseApiModel, new()
        {
            RestRequest request = new RestRequest(resource, Method.GET);
            request.RequestFormat = DataFormat.Json;

            if (!string.IsNullOrEmpty(token))
                request.AddHeader("x-access-token", token);

            if (parameters != null)
            {
                foreach (var keyValue in parameters)
                    request.AddQueryParameter(keyValue.Key, keyValue.Value);
            }

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT));
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

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT));
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

        #region Account Methods
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
            const string resource = @"account/";

            string body = JsonConvert.SerializeObject(new { username = user, email = email, password = password });

            return await PostAsync<BaseApiModel>(resource, body);
        }

        public static async Task<BaseApiModel> VerifyToken(string token)
        {
            const string resource = @"account/verifyToken";

            return await PostAsync<BaseApiModel>(resource, token: token);
        }
        #endregion
        #region Feed Methods
        public static async Task<BaseApiModel> UploadPost(string data, string category)
        {
            const string resource = @"feed/";

            string body = JsonConvert.SerializeObject(new { image = data, category = category });

            return await PostAsync<BaseApiModel>(resource, body, token: Token.value);
        }

        public static async Task<FeedModel> GetFeedPosts(int page)
        {
            const string resource = @"feed/";

            return await GetAsync<FeedModel>(resource, token: Token.value);
        }

        public static async Task<CategoryCollectionModel> GetPopularCategories()
        {
            const string resource = @"category/popular";

            return await GetAsync<CategoryCollectionModel>(resource, token: Token.value);
        }

        public static async Task<SearchSuggestionModel> GetSearchSuggestions(string text)
        {
            const string resource = @"category/autocomplete";

            var parameter = new KeyValuePair<string, string>("search", text);

            return await GetAsync<SearchSuggestionModel>(resource,Token.value, parameter);
        }

        public static async Task<CategoryCollectionModel> SearchCategory(string category)
        {
            const string resource = @"category/search";

            var parameter = new KeyValuePair<string, string>("category", category);

            return await GetAsync<CategoryCollectionModel>(resource, Token.value,parameter);
        }

        public static async Task<SingleCategoryModel> GetCategoryPosts(string categoryId)
        {
            const string resource = @"category";

            var parameter = new KeyValuePair<string, string>("categoryId", categoryId);

            return await GetAsync<SingleCategoryModel>(resource, Token.value,parameter);
        }

        public static async Task<FollowModel> GetFollowStatus(string categoryId)
        {
            const string resource = @"category/follow";

            var parameter = new KeyValuePair<string, string>("categoryId", categoryId);

            return await GetAsync<FollowModel>(resource, Token.value,parameter);
        }

        public static async Task<BaseApiModel> FollowCategory(string categoryId)
        {
            const string resource = @"category/follow";

            string body = JsonConvert.SerializeObject(new { categoryId = categoryId });

            return await PostAsync<BaseApiModel>(resource, body, token: Token.value);
        }

        public static async Task<SinglePostModel> GetPostDetail(string postId)
        {
            const string resource = @"feed/detail";

            var parameter = new KeyValuePair<string, string>("postId", postId);

            return await GetAsync<SinglePostModel>(resource, Token.value,parameter);
        }

        public static async Task<VoteModel> GetVoteStatus(string postId)
        {
            const string resource = @"feed/vote";

            var parameter = new KeyValuePair<string, string>("postId", postId);

            return await GetAsync<VoteModel>(resource, Token.value,parameter);
        }

        public static async Task<VoteModel> VotePost(string postId, int vote)
        {
            const string resource = @"feed/vote";

            string body = JsonConvert.SerializeObject(new { postId = postId, vote = vote });

            return await PostAsync<VoteModel>(resource, body, token: Token.value);
        }

        public static async Task<PostCollectionModel> GetProfilePosts()
        {
            const string resource = @"feed/profile";

            return await GetAsync<PostCollectionModel>(resource, Token.value);
        }


        #endregion
    }
}
