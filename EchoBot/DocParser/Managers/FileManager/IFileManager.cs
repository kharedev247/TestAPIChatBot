using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocParser.Managers.FileManager
{
    public interface IFileManager
    {
        Task<OpenApiDocument> GetAPIObject(string fileName);
    }
}
