using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Handlers
{
    public class GetApiPathHandler
    {
        public static List<string> AvailablePhysicalPath(OpenApiDocument openApiDocument)
        {
            if(openApiDocument == null)
            {
                throw new ArgumentNullException("Can not be null:" + openApiDocument);
            }
            List<string> paths = new List<string>();
            foreach (KeyValuePair<string, OpenApiPathItem> pathInfo in openApiDocument.Paths)
            {
                foreach (KeyValuePair<OperationType,OpenApiOperation> operation in pathInfo.Value.Operations)
                {
                    var path = operation.Key.ToString() + "  " + pathInfo.Key;
                    paths.Add(path);
                }
            }
            return paths;
        }
    }
}
