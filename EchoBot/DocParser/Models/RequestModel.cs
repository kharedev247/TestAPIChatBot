using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocParser.Models
{
    public class RequestModel
    {
        public RequestModel(string sName,string aName,OperationType otype)
        {
            ServerName = sName;
            ApiName = aName;
            RequestType = otype;
        }
        public string ServerName { get; set; }
        public string ApiName { get; set; }
        public OperationType RequestType { get; set; }
        public Dictionary<string,string> RequestParamInPath { get; set; }
        public Dictionary<string,string> RequestParamInQuery { get; set; }
        public string ContentType { get; set; }
        public string RequestBody { get; set; }
    }
}
