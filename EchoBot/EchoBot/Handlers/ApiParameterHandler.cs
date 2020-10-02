using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace EchoBot.Handlers
{
    public interface IApiParameterHandler
    {
        Task<Dictionary<string, ParamInfo>> GetParams(string apiPath, OperationType operationType, OpenApiDocument apiDocument);
        Task<string> GetApiPath(string text);
        Task<OperationType> GetOperationType(string text);
    }

    public class ApiParameterHandler : IApiParameterHandler
    {
        public async Task<Dictionary<string, ParamInfo>> GetParams(string apiPath, OperationType operationType, OpenApiDocument apiDocument)
        {
            if (string.IsNullOrEmpty(apiPath) || apiDocument == null)
            {
                throw new ArgumentNullException("Arguments passed can not be null");
            }
            var apiOperations = apiDocument.Paths[apiPath];
            var apiParams = apiOperations.Operations[operationType].Parameters;

            Dictionary<string, ParamInfo> dic = new Dictionary<string, ParamInfo>();

            foreach (var param in apiParams)
            {
                var paramName = "{" + param.Name + "}";
                ParamInfo paramInfo = new ParamInfo(paramName, param.Description, param.Schema.Type, param.In.ToString(), param.Required);
                dic.Add(paramName, paramInfo);                
            }
            return dic;
        }

        public async Task<OperationType> GetOperationType(string apiPath)
        {
            var reqType = apiPath.Substring(0, apiPath.IndexOf(" "));
            switch (reqType)
            {   
                case "Get": return OperationType.Get;
                case "Put": return OperationType.Put;
                case "Post": return OperationType.Post;
                case "Delete": return OperationType.Delete;
                case "Options": return OperationType.Options;
                case "Head": return OperationType.Head;
                case "Patch": return OperationType.Patch;
                case "Trace": return OperationType.Trace;
                default:
                    throw new InvalidOperationException(reqType+"no Such Operation Found");
            }
        }

        public async Task<string> GetApiPath(string apiPath)
        {
            return apiPath.Substring(apiPath.IndexOf("/"));
        }
    }
}
