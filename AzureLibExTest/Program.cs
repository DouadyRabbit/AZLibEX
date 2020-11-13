using AZLibEx;
using System;

namespace AzureLibExTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AzConnectEx aZConnectEx = new AzConnectEx();

            aZConnectEx.DefaultEndpointsProtocol = "https";
            aZConnectEx.EndpointSuffix = "core.windows.net";
            aZConnectEx.AccountName = "";
            aZConnectEx.AccountKey = "";
            aZConnectEx.blobContainerName = "";

            string blobPattern = @"pictures/CompassionFatigue";
            string outPath = @"c:\delete";
            int rtnVal = 0;

            rtnVal = aZConnectEx.TransferAzureBlobFromPatternToDirectory(blobPattern, outPath);
            Console.ReadLine();

        }
    }
}
