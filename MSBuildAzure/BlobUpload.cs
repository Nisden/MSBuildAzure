namespace MSBuildAzure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Framework;
    using System.Text;
    using Microsoft.Build.Utilities;
    using Microsoft.WindowsAzure.Storage;
    using System.Xml.Serialization;
    using System.IO;

    [Serializable]
    public class BlobUpload : Task, ITask
    {
        [Required]
        [XmlAttribute]
        public string ConnectionString { get; set; }

        [Required]
        [XmlAttribute]
        public string Container { get; set; }

        [XmlAttribute]
        public bool CreateContainer { get; set; }

        [XmlAttribute]
        public string ContentType { get; set; }

        [XmlAttribute]
        public string CacheControl { get; set; }

        [Required]
        [XmlAttribute]
        public ITaskItem[] Files { get; set; }

        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentNullException("ConnectionString");

            if (string.IsNullOrWhiteSpace(Container))
                throw new ArgumentNullException("Container");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(Container);

            if (CreateContainer)
            {
                Log.LogMessage("Creating azure container");
                blobContainer.CreateIfNotExists();
            }

            foreach (var fileItem in Files)
            {
                FileInfo file = new FileInfo(fileItem.ItemSpec);
                Log.LogMessage("Uploading file: {0}", file.FullName);

                var blob = blobContainer.GetBlockBlobReference(file.Name);

                using (Stream fileStream = file.OpenRead())
                {
                    blob.UploadFromStream(fileStream);
                }

                if (!string.IsNullOrEmpty(CacheControl))
                    blob.Properties.CacheControl = CacheControl;

                if (!string.IsNullOrEmpty(ContentType))
                    blob.Properties.ContentType = ContentType;

                blob.SetProperties();
            }

            return true;
        }
    }
}
