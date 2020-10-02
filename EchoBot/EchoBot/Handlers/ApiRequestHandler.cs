using DocParser.Managers.RequestManager;
using DocParser.Models;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EchoBot.Handlers
{
    public interface IApiRequestHandler
    {
        Task<ResponseModel> SendApiRequest(OpenApiDocument openApiDocument, ConversationData userConversation);
    }
    public class ApiRequestHandler : IApiRequestHandler
    {
        public ApiRequestHandler(IRequestManager requestManager)
        {
            RequestManager = requestManager;
        }

        public IRequestManager RequestManager { get; }

        public async Task<ResponseModel> SendApiRequest(OpenApiDocument openApiDocument, ConversationData userConversation)
        {
            var requestModel = new RequestModel(openApiDocument.Servers[0].Url, userConversation.ApiPath, userConversation.OperationType);
			requestModel.RequestParamInPath = userConversation.ParamsInfo.Where((parameter) => parameter.Value.ParamIn.Equals("path")).ToDictionary(key => key.Value.ParamName, value => value.Value.ParamValue);
			requestModel.RequestParamInQuery = userConversation.ParamsInfo.Where((parameter) => parameter.Value.ParamIn.Equals("query")).ToDictionary(key => key.Value.ParamName, value => value.Value.ParamValue);
            if(userConversation.OperationType == OperationType.Post && userConversation.RequestBody.Count > 0)
            {
                requestModel.ContentType = "application/json";
                requestModel.RequestBody = userConversation.RequestBody["userBody"];
            }
			// For testing SendAPIRequest
			//UpdateRequestModelForDummyAPITest(requestModel);

			return await RequestManager.SendRequestAsync(requestModel);
        }

        // making dummy request model for testing
        private static void UpdateRequestModelForDummyAPITest(RequestModel requestModel)
        {
            requestModel.ServerName = "http://demo7451219.mockable.io/";
            requestModel.ApiName = "mygettest/{param1}/{param2}";
            requestModel.RequestParamInPath = new Dictionary<string, string>();
            requestModel.RequestParamInPath.Add("{param1}", "firstparam");
            requestModel.RequestParamInPath.Add("{param2}", "secondparam");
            requestModel.RequestParamInQuery = new Dictionary<string, string>();
            requestModel.RequestParamInQuery.Add("userid", "admin");
            requestModel.RequestParamInQuery.Add("theme", "nova");
        }

        private string RemoveOperationType(string apiPath)
        {
            return apiPath.Substring(apiPath.IndexOf("/"));
        }
    }
}
