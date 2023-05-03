using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System.IO;
using System.Text;

namespace LargeDataSet
{
    public class Blob
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Blob(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public string? UpLoad(string requestId, string json, bool returnSas)
        {
            var blobServiceClient = new BlobServiceClient(
                new Uri(_configuration["Blob:Uri"]),
                ManagedIdentityCredentialHelper.GetCredential(_environment));
            var containerClient = blobServiceClient.GetBlobContainerClient(_configuration["Blob:Container"]);
            var blobClient = containerClient.GetBlobClient($"{requestId}.json");

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            blobClient.Upload(stream);

            if (returnSas)
            {
                //now create a SAS token for the blob - storage account must have a public endpoint
                var userDelegationKey = blobServiceClient.GetUserDelegationKey(DateTimeOffset.UtcNow,
                                                                        DateTimeOffset.UtcNow.AddMinutes(10));

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b", // b for blob, c for container
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(2),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
                {
                    Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName)
                };

                return blobUriBuilder.ToUri().ToString();
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> Download(string requestId)
        {
            var blobServiceClient = new BlobServiceClient(
                new Uri(_configuration["Blob:Uri"]),
                ManagedIdentityCredentialHelper.GetCredential(_environment));
            var containerClient = blobServiceClient.GetBlobContainerClient(_configuration["Blob:Container"]);
            var blobClient = containerClient.GetBlobClient($"{requestId}.json");

            BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
            string blobContents = downloadResult.Content.ToString();

            return blobContents;

        }
    }
}
