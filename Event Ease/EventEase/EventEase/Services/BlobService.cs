// Kaden Jason Remley ST10472838
// CLDV6211 POE Part 1
// EventEase - Venue Booking System

// References:
// Microsoft (2026) Upload files to Azure Blob Storage with ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-upload
// (Accessed: 02 May 2026)
//
// Microsoft (2026) Azure Blob Storage client library for .NET.
// Available at: https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
// (Accessed: 02 May 2026)
//
// Microsoft (2026) Dependency injection in ASP.NET Core.
// Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection
// (Accessed: 02 May 2026)
//
// Dhanampal, T. (2026) CLDV6211 lecture notes on cloud storage integration.
// The Independent Institute of Education.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventEase.Services;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");

            var blobServiceClient = new BlobServiceClient(connectionString);

            _containerClient = blobServiceClient.GetBlobContainerClient("venue-images");

            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobClient = _containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }
}