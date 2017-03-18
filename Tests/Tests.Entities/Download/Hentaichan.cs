using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Download
{
  [TestClass]
  public class HentaichanDL
  {
    [TestMethod]
    public async Task DownloadHentaichan()
    {
      CreateLogin();
      var rm = Mangas.Create(new Uri(@"http://henchan.me/related/8824-o-ore-no-imouto-gaa-glava-3.5.html"));
      var sw = new Stopwatch();
      sw.Start();
      await rm.Download();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}");
      Assert.IsTrue(Directory.Exists(rm.Folder));
      var files = Directory.GetFiles(rm.Folder, "*", SearchOption.AllDirectories);
      Assert.AreEqual(139, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(93596158, fileInfos.Sum(f => f.Length));
      Assert.AreEqual(rm.Chapters.Count, fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(rm.IsDownloaded);
    }

    private void CreateLogin()
    {
      var setting = ConfigStorage.GetPlugin<Hentaichan.Hentaichan>().GetSettings();
      var login = setting.Login as Hentaichan.HentaichanLogin;
      login.UserId = "235332";
      login.PasswordHash = "0578caacc02411f8c9a1a0af31b3befa";
      login.IsLogined = true;
      setting.Save();
    }

  }
}
