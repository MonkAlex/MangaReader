using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Download
{
  [TestFixture]
  public class ReadmangaDL : TestClass
  {
    private int lastPercent = 0;

    [Test]
    public async Task DownloadReadmanga()
    {
      var rm = Mangas.CreateFromWeb(new Uri(@"http://readmanga.me/black_butler_anthology_comic_dj____rainbow_butler"));
      var sw = new Stopwatch();
      sw.Start();
      rm.DownloadProgressChanged += RmOnDownloadProgressChanged;
      DirectoryHelpers.DeleteDirectory(rm.GetAbsoulteFolderPath());
      await rm.Download();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}, iscompleted = {rm.IsDownloaded}, lastpercent = {lastPercent}");
      Assert.IsTrue(Directory.Exists(rm.GetAbsoulteFolderPath()));
      var files = Directory.GetFiles(rm.GetAbsoulteFolderPath(), "*", SearchOption.AllDirectories);
      Assert.AreEqual(212, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(67470935, fileInfos.Sum(f => f.Length));

      // Количество повторяющихся картинок на мангу - не больше одной на главу.
      Assert.IsTrue(rm.Volumes.Sum(v => v.Container.Count()) >= fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(rm.IsDownloaded);
      Assert.AreEqual(100, lastPercent);
    }

    private void RmOnDownloadProgressChanged(object sender, IManga manga)
    {
      var dl = (int) manga.Downloaded;
      if (dl > lastPercent)
        lastPercent = dl;
    }
  }
}
