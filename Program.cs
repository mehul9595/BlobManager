using System;
using Microsoft.Extensions.Configuration;
using Azure.Storage;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using System.IO;

namespace BlobManagerApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path = Path.GetFullPath(@"appsettings.json");
            Console.WriteLine($"{path}");
            IConfiguration configuration = new ConfigurationBuilder()
                                                .AddJsonFile(path).Build();

            Console.Write($"{configuration["storageAccountName"]}");

            Console.WriteLine("Hello World!");
        }
    }
}
