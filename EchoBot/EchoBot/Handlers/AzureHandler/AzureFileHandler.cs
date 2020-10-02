using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace EchoBot.Handlers.AzureHandler
{
    public class AzureFileHandler : IAzureFileHandler
	{
		public async Task<bool> UploadFileAsync(string filePath, string fileName, string storageConnection) {
			try
			{
				CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

				CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

				CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("apidocs");

				//create a container if it is not already exists

				if (await cloudBlobContainer.CreateIfNotExistsAsync())
				{

					await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

				}

				var fileToUpload = Path.Combine(filePath);
				string imageName = fileName + Path.GetExtension(fileToUpload);


				CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);

                await cloudBlockBlob.UploadFromFileAsync(fileToUpload);

                //await cloudBlockBlob.UploadFromStreamAsync(dataStream);

                return true;
			}
			catch (Exception e) {
                //throw e;
                return false;
			}
		}
		
	}
}
