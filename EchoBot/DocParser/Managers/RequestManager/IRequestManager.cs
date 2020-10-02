using DocParser.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DocParser.Managers.RequestManager
{
    public interface IRequestManager
    {
        Task<ResponseModel> SendRequestAsync(RequestModel requestModel);
        string AddParamWithRequestPath(string apiPath, Dictionary<string, string> requestParam);
        Task<ResponseModel> GetApiResponse(HttpClient client, Uri apiPath);

    }
}