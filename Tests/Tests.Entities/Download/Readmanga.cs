using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Download
{
  [TestClass]
  public class ReadmangaDL
  {
    [TestMethod]
    public void DownloadReadmanga()
    {
      // var rm = Mangas.Create(new Uri(@"http://henchan.me/related/15692-sweet-guy-glava-0-prolog.html"));
      var rm = Mangas.Create(new Uri(@"http://readmanga.me/hack__xxxx"));
      var sw = new Stopwatch();
      sw.Start();
      rm.Download().Wait();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}");
      Assert.IsTrue(Directory.Exists(rm.Folder));
      var files = Directory.GetFiles(rm.Folder, "*", SearchOption.AllDirectories);
      Assert.AreEqual(249, files.Length);
      var size = files.Sum(f => new FileInfo(f).Length);
      Assert.AreEqual(64025297, size);
    }
  }
}
