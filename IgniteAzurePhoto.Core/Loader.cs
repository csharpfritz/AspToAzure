using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IgniteAzurePhoto.Core
{

  /// <summary>
  /// Loads the files to Azure storage
  /// </summary>
  public class Loader
  {
    private bool _StopWorking;
    private string _StorageConnString;
    private Queue<string> _WorkQueue;

    public Loader(Queue<string> workQueue, string connString)
    {

      _WorkQueue = workQueue;
      _StorageConnString = connString;

    }

    public async void BeginWorking()
    {

      _StopWorking = false;

      while (!_StopWorking)
      {

        if (_WorkQueue.Count() > 0)
        {

          Parallel.For(0,4, i =>
          {

            string workingImg;
            try {
              workingImg = _WorkQueue.Dequeue();
              if (string.IsNullOrEmpty(workingImg)) return;
            } catch (Exception)
            {
              return;
            }

            Upload(workingImg);

          });

        }

        await Task.Delay(1000);

      }


    }

    private void Upload(string workingImg)
    {

      Debug.WriteLine("-->> Uploading {0}", new string[] { workingImg });

      var datePart = DateTime.Now.ToString("HHmmss");

      var acct = CloudStorageAccount.Parse(_StorageConnString);
      var client = acct.CreateCloudBlobClient();
      var container = client.GetContainerReference("fullsize");

      var newFileName = string.Concat(datePart, "_", Path.GetFileName( workingImg));
      var newBlob = container.GetBlockBlobReference(newFileName);

      using (var fs = File.OpenRead(workingImg))
      {
        newBlob.UploadFromStream(fs);
        newBlob.Properties.ContentType = "image/jpeg";
      }


      Debug.WriteLine("-->> Completed Uploading {0}", new string[] { workingImg });

    }

    public void Stop()
    {
      _StopWorking = true;
    }



  }

}
