using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IgniteAzurePhoto.Core
{


  public class Watcher
  {

    private string _FolderLocal;
    private string _FolderWatch;
    private static bool _StopNow = false;
    private Queue<string> _WorkQueue;

    public Watcher(string folderToWatch, string localFolder, Queue<string> workQueue)
    {

      _FolderWatch = folderToWatch;
      _FolderLocal = localFolder;
      _WorkQueue = workQueue;

      var local = new DirectoryInfo(localFolder);
      if (!local.Exists)
      {
        local.Create();
      }

    }

    public async void BeginWatching()
    {

      _StopNow = false;

      while (!_StopNow)
      {

        if (IsWatchFolderAvailable)
        {

          Debug.WriteLine("- Folder Found");

          if (NewFilesAvailable)
          {

            Debug.WriteLine("-- New Files Found!");

            var watchFolder = new DirectoryInfo(_FolderWatch);
            Movefiles(watchFolder);

            Debug.WriteLine("<== ALL FILES MOVED ==>");
             
          }
          else
          {
            Debug.WriteLine("- No new files available");
          }

        } else
        {
          Debug.WriteLine("Folder not available");
        }

        await Task.Delay(2000);

      }

    }

    private void Movefiles(DirectoryInfo watchFolder)
    {

      foreach (var item in watchFolder.GetFiles().Where(f => f.Name.ToLowerInvariant().EndsWith("jpg") || f.Name.ToLowerInvariant().EndsWith("png")))
      {
        Debug.WriteLine(">>>> Moving {0}", new[] { item.Name });
        var localFilename = Path.Combine(_FolderLocal, item.Name);
        item.MoveTo(localFilename);
        _WorkQueue.Enqueue(localFilename);
      }

      foreach (var item in watchFolder.GetDirectories())
      {
        Movefiles(item);
      }

    }

    public bool IsWatchFolderAvailable
    {
      get {

        try
        {
          return System.IO.Directory.Exists(_FolderWatch);
        }
        catch (Exception)
        {
          return false;
        }

      }
    }

    public bool NewFilesAvailable
    {
      get
      {

        return FirstWatchDateTime > LastLocalWriteDateTime;

      }
    }

    public DateTime FirstWatchDateTime
    {
      get
      {

        var watchFolder = new DirectoryInfo(_FolderWatch);
        var files = watchFolder.GetFiles().OrderBy(f => f.CreationTimeUtc);

        return files.Count() == 0 ? DateTime.MinValue : files.First().CreationTimeUtc;

      }
    }

    public DateTime LastLocalWriteDateTime
    {
      get
      {

        var localFolder = new DirectoryInfo(_FolderLocal);
        var files = localFolder.GetFiles().OrderByDescending(f => f.CreationTimeUtc);

        return files.Count() == 0 ? DateTime.MinValue : files.First().CreationTimeUtc;

      }
    }

    public void Stop() {
      _StopNow = true;
    }


  }

}
