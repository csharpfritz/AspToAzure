using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteAzurePhoto.Core
{

  public class Config
  {
    private Config() { }

    public static Config Get()
    {

      var cfg = new Config()
      {
        WatchLocation = ConfigurationManager.AppSettings.Get("watchLocation"),
        LocalLocation = ConfigurationManager.AppSettings.Get("localLocation"),
        StorageConnString = ConfigurationManager.AppSettings.Get("StorageConnectionString")
      };

      return cfg;

    }

    public string WatchLocation { get; set; }

    public string LocalLocation { get; set; }

      public string StorageConnString { get; private set; }

}

}
