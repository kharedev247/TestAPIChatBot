using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot
{
    public class UserProfile
    {
        public string Name { get; set; }
        public OpenApiDocument ApiObject { get; set; }
        public string ApiPath { get; set; }
    }
}
