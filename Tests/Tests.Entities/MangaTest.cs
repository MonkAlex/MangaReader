using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using Tests.Entities;

namespace Tests
{
  public class MangaTest : TestClass
  {
    private static MangaInfo[] MangaToDownload()
    {
      MangaInfos.Init();
      return new[]
      {
        MangaInfos.Acomics.MgsLdioh,
        MangaInfos.Henchan.TwistedIntent,
        MangaInfos.Hentai2Read.AttentionPlease,
        MangaInfos.Mangachan.Rain,
        MangaInfos.Mintmanga.HarukaNaReceive,
        MangaInfos.Readmanga.Kuroshitsuji
      };
    }

    private static TestCaseData[] MangaToValidateStatusAndDescription()
    {
      MangaInfos.Init();
      return new[]
      {
        new TestCaseData(MangaInfos.Acomics.SuperScienceFriends),
        new TestCaseData(MangaInfos.Henchan.LoveAndDevil),
        new TestCaseData(MangaInfos.Hentai2Read.AttentionPlease),
        new TestCaseData(MangaInfos.Mangachan.ThisGirlfriendIsFiction),
        new TestCaseData(MangaInfos.Mangachan.EveScramble).SetDescription("html codes in description"),
        new TestCaseData(MangaInfos.Mintmanga.LoveMate),
        new TestCaseData(MangaInfos.Readmanga.Kuroshitsuji),
      };
    }

    [Test, TestCaseSource(nameof(MangaToDownload))]
    public async Task Download(MangaInfo mangaInfo)
    {
      IManga manga;
      var sw = new Stopwatch();
      using (Repository.GetEntityContext($"Test to download {mangaInfo.Uri}"))
      {
        manga = await Mangas.CreateFromWeb(new Uri(mangaInfo.Uri));
        sw.Start();
        DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
        await manga.Download();
      }

      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}, iscompleted = {manga.IsDownloaded}, downloaded = {manga.Downloaded}%");
      Assert.IsTrue(Directory.Exists(manga.GetAbsoluteFolderPath()));
      var files = Directory.GetFiles(manga.GetAbsoluteFolderPath(), "*", SearchOption.AllDirectories);
      Assert.AreEqual(mangaInfo.FilesInFolder, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(mangaInfo.FolderSize, fileInfos.Sum(f => f.Length));
      if (mangaInfo.AllFilesUnique)
        Assert.AreEqual(1, fileInfos.GroupBy(f => f.Length).Max(g => g.Count()));
      Assert.IsTrue(manga.IsDownloaded);
      Assert.AreEqual(100, manga.Downloaded);
    }

    [Test, TestCaseSource(nameof(MangaToValidateStatusAndDescription))]
    public async Task ValidateStatusAndDescription(MangaInfo mangaInfo)
    {
      IManga manga;
      using (var context = Repository.GetEntityContext("Description"))
      {
        var mangaUri = new Uri(mangaInfo.Uri);
        var existsManga = context.Get<IManga>().FirstOrDefault(m => m.Uri == mangaUri);
        if (existsManga != null)
          context.Delete(existsManga);
        manga = await Mangas.CreateFromWeb(mangaUri);
      }

      Assert.AreEqual(mangaInfo.Description, manga.Description);
      if (Equals(mangaInfo.Status, manga.Status))
        Assert.Pass();
      else
      {
        // Status can contain regular chagned info, try to compare
        Assert.Less(LevenshteinDistance(mangaInfo.Status, manga.Status), 50);
      }
    }

    [Test, Ignore("Only for generate actual manga info (and check diff)")]
    public async Task GenerateInfoForTests()
    {
      var fields = typeof(MangaInfos).GetNestedTypes().SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public));
      var cacheAttributes = fields.Select(f => f.GetCustomAttribute<InfoCacheAttribute>()).ToList();
      var tasks = cacheAttributes.Select(u => Builder.Generate(u)).ToArray();
      await Task.WhenAll(tasks);
      var infos = tasks.Select(t => t.Result).OrderBy(i => i.Uri).ToList();
      var json = JsonConvert.SerializeObject(infos);
      File.WriteAllText(Environment.MangaCache, json);
    }

    public static int LevenshteinDistance(string string1, string string2)
    {
      if (string1 == null) throw new ArgumentNullException("string1");
      if (string2 == null) throw new ArgumentNullException("string2");
      int diff;
      int[,] m = new int[string1.Length + 1, string2.Length + 1];

      for (int i = 0; i <= string1.Length; i++) { m[i, 0] = i; }
      for (int j = 0; j <= string2.Length; j++) { m[0, j] = j; }

      for (int i = 1; i <= string1.Length; i++)
      {
        for (int j = 1; j <= string2.Length; j++)
        {
          diff = (string1[i - 1] == string2[j - 1]) ? 0 : 1;

          m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1,
              m[i, j - 1] + 1),
            m[i - 1, j - 1] + diff);
        }
      }
      return m[string1.Length, string2.Length];
    }
  }
}