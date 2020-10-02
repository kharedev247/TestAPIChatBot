using DocParser.Managers.FileManager;
using DocParser.Managers.RequestManager;
using EchoBot.Handlers.AzureHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Handlers.FileHandler
{
    public class FileHandler :IFileHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureFileHandler _azureFileManager;
        private readonly IFileManager _fileManager;
        //private readonly IRequestManager _requestManager;
        public FileHandler(IConfiguration configuration, IAzureFileHandler azureFileManager, IFileManager fileManager)
        {
            _configuration = configuration;
            _azureFileManager = azureFileManager;
            _fileManager = fileManager;
            //_requestManager = requestManager;
        }

        public async Task<OpenApiDocument> SaveFileToLocal(string filePath, string fileName)
        {
            OpenApiDocument response = await _fileManager.GetAPIObject(fileName + Path.GetExtension(Path.Combine(filePath)));
            return response;
        }

		public async Task<OpenApiDocument> UploadFileToAzure(string filePath, string fileName)
		{
            try
            {
                ////Testing
                //_requestManager.GetApiResponse("test");
                var storageConnection = _configuration.GetSection("AzureKeys").GetSection("StorageConnection").Value;
                var isUploaded = await _azureFileManager.UploadFileAsync(filePath, fileName, storageConnection);
                if (isUploaded)
                {
                    OpenApiDocument response = await _fileManager.GetAPIObject(fileName + Path.GetExtension(Path.Combine(filePath)));
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
	}
}
