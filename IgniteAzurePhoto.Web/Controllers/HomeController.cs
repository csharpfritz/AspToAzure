using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;

namespace IgniteAzurePhoto.Web.Controllers
{
  public class HomeController : Controller
  {

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {

      ViewBag.UploadEnabled = UploadEnabled;
      base.OnActionExecuting(filterContext);

    }

    public bool UploadEnabled
    {
      get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UploadEnabled"]); }
    }

    [OutputCache(Duration =5)]
    public ActionResult Index()
    {

      
      var blobs = GetThumbnailBlobs();

      return View(blobs);

    }

    [OutputCache(Duration = 120)]
    public ActionResult About()
    {

      Trace.WriteLine("About page requested");

      return View();
    }

    [OutputCache(Duration = 30)]
    public ActionResult Upload()
    {
      if (UploadEnabled) return View();

      return new HttpNotFoundResult("Upload not currently enabled");

    }

    [HttpPost]
    public ActionResult Upload(HttpPostedFileBase photoToUpload)
    {

      if (!UploadEnabled)
        return new HttpNotFoundResult("Upload not currently enabled");

      if (!photoToUpload.ContentType.StartsWith("image/"))
      {
        throw new HttpRequestValidationException("Uploaded content was not an image");
      }

      Trace.Write("Beginning upload of " + photoToUpload.FileName + " to blob storage");

      UploadToBlob(photoToUpload);

      Trace.Write("Completed upload of " + photoToUpload.FileName + " to blob storage");

      return new JavaScriptResult();

    }

    internal static void UploadToBlob(HttpPostedFileBase photo)
    {

      var datePart = DateTime.Now.ToString("HHmmss");

      var acct = GetStorageAcct();
      var client = acct.CreateCloudBlobClient();
      var container = client.GetContainerReference("upload");

      var newFileName = string.Concat(datePart, "_", photo.FileName);
      var newBlob = container.GetBlockBlobReference(newFileName);

      using (var fs = photo.InputStream)
      {
        newBlob.UploadFromStream(fs);
        newBlob.Properties.ContentType = photo.ContentType;
      }


  }

  internal static IEnumerable<string> GetThumbnailBlobs()
    {

      var acct = GetStorageAcct();
      var client = acct.CreateCloudBlobClient();
      var container = client.GetContainerReference("thumbnail");

      return container.ListBlobs().Select(b => b.Uri.ToString()).ToList();

    }

    private static CloudStorageAccount GetStorageAcct()
    {
      return CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
    }
  }
}