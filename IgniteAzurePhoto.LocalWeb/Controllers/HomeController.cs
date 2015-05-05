using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.IO;

namespace IgniteAzurePhoto.Web.Controllers
{
  public class HomeController : Controller
  {

    [OutputCache(Duration = 5)]
    public ActionResult Index()
    {

      var imgs = GetImages();

      return View(imgs);

    }

    [OutputCache(Duration = 120)]
    public ActionResult About()
    {

      return View();
    }

    internal IEnumerable<string> GetImages()
    {

      var d = new DirectoryInfo(Server.MapPath("~/Photos"));

      return d.GetFiles().Select(f => "/Photos/" + f.Name);

    }

  }
}