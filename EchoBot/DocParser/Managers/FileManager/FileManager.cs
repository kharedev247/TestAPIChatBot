using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DocParser.Managers.FileManager
{
    public class FileManager : IFileManager
    {

        private readonly IConfiguration _configuration;

        public FileManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<OpenApiDocument> GetAPIObject(string fileName)
        {
            //string location = _configuration.GetSection("FileStroagePath").Value;

            // Dealing with local file storage as fetchinga and storing files on azure is taking time
            string location = _configuration.GetSection("PhysicalStoragePath").Value;


            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("Can not be null:" + fileName);
            }
            Stream stream = new WebClient().OpenRead(location + "\\" + fileName);
            OpenApiDocument document = new OpenApiStreamReader().Read(stream, out var diagnostic);

            return Task.FromResult(document);
        }
    }
}
