using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using ImageResizer.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IgniteAzurePhoto.WebJob
{
  // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
  class Program
  {

    // Please set the following connection strings in app.config for this WebJob to run:
    // AzureWebJobsDashboard and AzureWebJobsStorage
    static void Main()
    {
      var host = new JobHost();


      // The following code ensures that the WebJob will be running continuously
      host.RunAndBlock();

      //host.Call(typeof(Functions).GetMethod("ResizeThumbnail"));
      //host.Call(typeof(Functions).GetMethod("ResizeMidsize"));

    }


  }
}
