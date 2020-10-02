using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBot.Handlers.AzureHandler
{
	public interface IAzureFileHandler
	{
		  Task<bool> UploadFileAsync(string filePath, string fileName, string storageConnection);
	}
}
