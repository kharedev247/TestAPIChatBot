using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Handlers
{
	public interface IFileHandler
	{
		 Task<OpenApiDocument> SaveFileToLocal(string filePath, string fileName);
		 Task<OpenApiDocument> UploadFileToAzure(string filePath, string fileName);
	}
}
