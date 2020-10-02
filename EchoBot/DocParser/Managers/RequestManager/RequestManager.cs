using DocParser.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace DocParser.Managers.RequestManager
{
    public class RequestManager : IRequestManager
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RequestManager(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseModel> SendRequestAsync(RequestModel requestModel)
        {
            try
            {
                ResponseModel response = null;

                var apiPath = requestModel.ApiName;
                if (requestModel.RequestParamInPath.Count > 0)
                    apiPath = AddParamWithRequestPath(requestModel.ApiName, requestModel.RequestParamInPath);

                var client = _httpClientFactory.CreateClient();
                UriBuilder uriBuilder = new UriBuilder(requestModel.ServerName + apiPath);

                if (requestModel.RequestParamInQuery.Count > 0)
                {
                    AddParamInQuery(requestModel, uriBuilder);
                }

                client.BaseAddress = uriBuilder.Uri;

                if (OperationType.Get == requestModel.RequestType)
                {
                    response = await GetApiResponse(client, uriBuilder.Uri);
                }

                if(OperationType.Post == requestModel.RequestType)
                {
                    response = await PostApiResponse(client, uriBuilder.Uri, requestModel.RequestBody, requestModel.ContentType);
                }

                //if(OperationType.Put == requestModel.RequestType)
                //{

                //}

                if (OperationType.Delete == requestModel.RequestType)
                {
                    response = await DeleteApiResponse(client, uriBuilder.Uri);
                }

                if (response == null)
                {
                    return null;
                }

                return response;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static void AddParamInQuery(RequestModel requestModel, UriBuilder uriBuilder)
        {
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (KeyValuePair<string, string> keyValue in requestModel.RequestParamInQuery)
            {
                query[keyValue.Key] = keyValue.Value;
            }
            uriBuilder.Query = query.ToString();
        }

        public async Task<ResponseModel> GetApiResponse(HttpClient client, Uri apiPath)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                using (HttpResponseMessage res = await client.GetAsync(apiPath))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    else
                    {
                        responseModel.StatusCode = res.StatusCode.ToString();
                        responseModel.ResponseBody = await res.Content.ReadAsStringAsync();
                        return responseModel;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ResponseModel> PostApiResponse(HttpClient client, Uri apiPath, string requestBody, string contentType)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                var body = new StringContent(requestBody, Encoding.UTF8, contentType);
                using (HttpResponseMessage res = await client.PostAsync(apiPath, body))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    else
                    {
                        responseModel.StatusCode = res.StatusCode.ToString();
                        responseModel.ResponseBody = await res.Content.ReadAsStringAsync();
                        return responseModel;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseModel> DeleteApiResponse(HttpClient client, Uri apiPath)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                using (HttpResponseMessage res = await client.DeleteAsync(apiPath))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    else
                    {
                        responseModel.StatusCode = res.StatusCode.ToString();
                        responseModel.ResponseBody = "Deletion Successful";
                        return responseModel;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddParamWithRequestPath(string apiPath, Dictionary<string,string> requestParamInPath)
        {
            var words = apiPath.Split("/");
            for (int i = 0; i < words.Length; i++)
            {
                Regex rgx = new Regex(@"\{([^)]+)\}");
                if (rgx.IsMatch(words[i]))
                {
                    words[i] = requestParamInPath[words[i]];
                }
            }

            return string.Join('/', words);
        }
    }
}
