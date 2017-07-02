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
  public class AcomicsDL : TestClass
  {
    [Test]
    public async Task DownloadAcomics()
    {
      var rm = Mangas.Create(new Uri(@"https://acomics.ru/~MGS-LDIOH"));
      var sw = new Stopwatch();
      sw.Start();
      await rm.Download();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}");
      Assert.IsTrue(Directory.Exists(rm.GetAbsoulteFolderPath()));
      var files = Directory.GetFiles(rm.GetAbsoulteFolderPath(), "*", SearchOption.AllDirectories);
      Assert.AreEqual(6, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(8301123, fileInfos.Sum(f => f.Length));
      Assert.AreEqual(1, fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(rm.IsDownloaded);
    }
  }
}
