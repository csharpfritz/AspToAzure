using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
  public class Functions
  {
    private static Config _Config;

    static Functions()
    {
      // Warm up the image resizer object
      var ar = new ImageResizer.Plugins.Basic.AutoRotate();
      _Config = new ImageResizer.Configuration.Config();
      ar.Install(_Config);
      
    }

    public static void ResizeThumbnail(
      [BlobTrigger(@"upload/{name}")] Stream inputStream,
      [Blob(@"thumbnail/{name}", FileAccess.Write)] Stream thumbnailBlob)
    {

      // Thumbnail
      var outImage = _Config.CurrentImageBuilder.Build(inputStream, new ImageResizer.ResizeSettings(@"maxwidth=200&maxheight=200&format=jpg&autorotate=true"));
      outImage.Save(thumbnailBlob, ImageFormat.Jpeg);
    }

    public static void ResizeMidsize(
      [BlobTrigger(@"upload/{name}")] Stream inputStream,
      [Blob(@"full/{name}", FileAccess.Write)] Stream midsizeBlob)
    {

      // midsize
      var midImage = _Config.CurrentImageBuilder.Build(inputStream, new ImageResizer.ResizeSettings(@"maxwidth=1200&maxheight=1200&format=jpg&autorotate=true"));
      midImage.Save(midsizeBlob, ImageFormat.Jpeg);

    }

  }
}
