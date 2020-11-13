using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AZLibEx
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("AZLibEx.AZConnectEx")]
    public class AzConnectEx
    {
        
        //-------------------Needed-----------------------------
        public string DefaultEndpointsProtocol = string.Empty;
        public string AccountName = string.Empty;
        public string AccountKey = string.Empty;
        public string EndpointSuffix = string.Empty;
        public string storageConnectionString = string.Empty;
        //--------------------------------------------------------


        public string errorMsg = string.Empty;
        public int errorNumber = 0;
        public decimal bytesUploaded = 0;
        
        public bool uploadCompleted = false;
        public bool waitToFinish = false;
        public int numberOfParallelOperations = 64;
        
        public string blobContainerName = string.Empty;
        public decimal blobSize = 0;
        public double sharedLinkLifeSpanDays = 3;

        public int percentageProgress = 0;

        public AzConnectEx()
        {
            
        }

        public int TransferAzureBlobFromPatternToDirectory(string prefixPattern, string destPath)
        {
            
            string continuationToken = null;
            int rtnVal = SetParameters();

            int? segmentSize = null;

            if (rtnVal != 0)
            {
                return rtnVal;
            }

            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnectionString, blobContainerName);

            try
            {
                // Call the listing operation and enumerate the result segment.
                // When the continuation token is empty, the last segment has been returned
                // and execution can exit the loop.
                do
                {
                    var resultSegment = blobContainerClient.GetBlobs(prefix: prefixPattern).AsPages(continuationToken, segmentSize);
                    

                    foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                    {
                        foreach (BlobItem blobItem in blobPage.Values)
                        {
                            //Console.WriteLine("Blob name: {0}", blobItem.Name);
                            BlobClient blobClient = new BlobClient(storageConnectionString, blobContainerName, blobItem.Name);

                            // Download the blob's contents and save it to a file
                            BlobDownloadInfo download = blobClient.Download();
                            string outFileName = Path.Combine(destPath, Path.GetFileName(blobItem.Name));

                            using (FileStream file = File.OpenWrite($"{outFileName}"))
                            {
                                download.Content.CopyTo(file);
                            }
                        }

                        // Get the continuation token and loop until it is empty.
                        continuationToken = blobPage.ContinuationToken;
                        //Console.WriteLine();
                    }

                } while (continuationToken != "");

            }
            catch (RequestFailedException e)
            {
                errorMsg = e.Message;
                //Console.WriteLine(e.Message);
                //Console.ReadLine();
                //throw;
            }

            return rtnVal;
        }


        //--------------------------------------------------------------------------------------------------------//
        public int SetParameters()
        {
            int rtnVal = 0;

            if (storageConnectionString == string.Empty || storageConnectionString.Trim().Length == 0)
            {
                rtnVal = CheckParameters();

                if (rtnVal != 0)
                {
                    return rtnVal;
                }
                else
                {
                    storageConnectionString = "DefaultEndpointsProtocol=" + DefaultEndpointsProtocol.Trim() + ";";
                    storageConnectionString += "AccountName=" + AccountName.Trim() + ";";
                    storageConnectionString += "AccountKey=" + AccountKey.Trim() + ";";
                    storageConnectionString += "EndpointSuffix=" + EndpointSuffix.Trim();
                }
            }

            if (rtnVal != 0)
            {
                return rtnVal;
            }
            return rtnVal;
        }

        //--------------------------------------------------------------------------------------------------------//
        private int CheckParameters()
        {
            int rtnVal = 0;

            if (DefaultEndpointsProtocol == string.Empty || DefaultEndpointsProtocol.Trim().Length == 0)
            {
                this.errorMsg += "Property DefaultEndPointProtocol cannot be empty" + Environment.NewLine;
                rtnVal = -1;
            }

            if (AccountName == string.Empty || AccountName.Trim().Length == 0)
            {
                this.errorMsg += "Property AccountName cannot be empty" + Environment.NewLine;
                rtnVal = -1;
            }

            if (AccountKey == string.Empty || AccountKey.Trim().Length == 0)
            {
                this.errorMsg += "Property AccountKey cannot be empty" + Environment.NewLine;
                rtnVal = -1;
            }

            if (EndpointSuffix == string.Empty || EndpointSuffix.Trim().Length == 0)
            {
                this.errorMsg += "Property EndpointSuffix cannot be empty" + Environment.NewLine;
                rtnVal = -1;
            }

            if (blobContainerName == string.Empty || blobContainerName.Trim().Length == 0)
            {
                this.errorMsg += "Property blobContainerName cannot be empty" + Environment.NewLine;
                rtnVal = -1;
            }

            return rtnVal;
        }

    }
}
