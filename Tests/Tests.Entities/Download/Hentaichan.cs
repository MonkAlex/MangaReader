using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NUnit.Framework;

namespace Tests.Entities.Download
{
  [TestFixture]
  public class HentaichanDL : TestClass
  {
    private int lastPercent = 0;

    [Test]
    public async Task DownloadHentaichan()
    {
      CreateLogin();
      var rm = Mangas.CreateFromWeb(new Uri(@"http://hentai-chan.me/related/12850-twisted-intent-chast-1.html"));
      var sw = new Stopwatch();
      sw.Start();
      rm.PropertyChanged += RmOnDownloadChanged;
      DirectoryHelpers.DeleteDirectory(rm.GetAbsoulteFolderPath());
      await rm.Download();
      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}, iscompleted = {rm.IsDownloaded}, lastpercent = {lastPercent}");
      Assert.IsTrue(Directory.Exists(rm.GetAbsoulteFolderPath()));
      var files = Directory.GetFiles(rm.GetAbsoulteFolderPath(), "*", SearchOption.AllDirectories);
      Assert.AreEqual(32, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(9864970, fileInfos.Sum(f => f.Length));
      Assert.AreEqual(1, fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(rm.IsDownloaded);
      Assert.AreEqual(100, lastPercent);
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

    private void RmOnDownloadChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(IManga.Downloaded))
      {
        var dl = (int)((IManga)sender).Downloaded;
        if (dl > lastPercent)
          lastPercent = dl;
      }
    }
  }
}
