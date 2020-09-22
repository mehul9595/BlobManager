using System;
using Microsoft.Extensions.Configuration;
using Azure.Storage;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using System.IO;
using Azure.Storage.Blobs.Models;

namespace BlobManagerApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path = Path.GetFullPath(@"appsettings.json");
            
            IConfiguration configuration = new ConfigurationBuilder()
                                                .AddJsonFile(path).Build();

            StorageSharedKeyCredential accountCred = new StorageSharedKeyCredential(configuration["storageAccountName"], configuration["storageAccountKey"]);
            BlobServiceClient blobClient = new BlobServiceClient(new Uri(configuration["blobConnectionEndpoint"]), accountCred);
            AccountInfo accountInfo = await blobClient.GetAccountInfoAsync();
            
            await Console.Out.WriteLineAsync($"Connected to Azure Storage Account");
            await Console.Out.WriteLineAsync($"Account name:\t{configuration["storageAccountName"]}");
            await Console.Out.WriteLineAsync($"Account kind:\t{accountInfo?.AccountKind}");
            await Console.Out.WriteLineAsync($"Account sku:\t{accountInfo?.SkuName}");

            await EnumerateBlobContainerAsync(blobClient);
            // await EnumerateBlobsAsync(blobClient, "rastor-graphics");
            // await GetOrAddContainerAsync(blobClient, "vector");

            Console.WriteLine("Hello World!");
        }

        static async Task EnumerateBlobContainerAsync(BlobServiceClient client)
        {
            int i = 1;

            await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
            {
                await Console.Out.WriteLineAsync($"Container {i++}:\t{container.Name}");

                await EnumerateBlobsAsync(client, container.Name);
            }
        }

        private static async Task EnumerateBlobsAsync(BlobServiceClient client, string containerName)
        {
            BlobContainerClient container = client.GetBlobContainerClient(containerName);

            await Console.Out.WriteLineAsync($"Searching:\t{container.Name}");

            int i = 1; 

            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                BlobClient blb = await GetBlobAsync(container, blob.Name);
                Console.Out.WriteLine($"{blob.Name} {i++} \t\t\t Uri: {blb.Uri}");
            }                  
        }

        private static async Task<BlobContainerClient> GetOrAddContainerAsync(BlobServiceClient client, string containerName)
        {      
            BlobContainerClient container = client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            await Console.Out.WriteLineAsync($"New Container:\t{container.Name}");
            return container;
        }


        static async Task<BlobClient> GetBlobAsync(BlobContainerClient client, string blobName )
        {
            BlobClient blob  = client.GetBlobClient(blobName);

            await Console.Out.WriteLineAsync($"Blob Found:\t{blob.Name}");
            return blob;
        }
        
    }
}
