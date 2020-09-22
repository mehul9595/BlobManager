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

            Console.WriteLine("Hello World!");
        }

        static async Task EnumerateBlobContainerAsync(BlobServiceClient client)
        {
            await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
            {
                await Console.Out.WriteLineAsync($"Container:\t{container.Name}");
            }
        }
    }
}
