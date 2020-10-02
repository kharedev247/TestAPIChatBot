using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot
{
    public class ConversationData
    {
        public string LastMessage;
        public Dictionary<string, ParamInfo> ParamsInfo { get; set; }
        public List<ParamInfo> RequestParams { get; set; }
        public Dictionary<string, string> RequestBody { get; set; } 
        public OperationType OperationType { get; set; }
        public string ApiPath { get; set; }
    }

    [Serializable]
    public class ParamInfo
    {
        public ParamInfo(string name, string description, string type, string paramIn, bool isRequired)
        {
            ParamName = name;
            ParamDescription = description;
            ParamType = type;
            ParamIn = paramIn;
            IsRequired = isRequired;
        }

		public ParamInfo() {}
        public string ParamName { get; set; }
        public string ParamDescription { get; set; }
        public string ParamType { get; set; }
        public string ParamValue { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;
        public string ParamIn { get; set; }
    }
}
