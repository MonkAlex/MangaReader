using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hentaichan.Mangachan;
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
        manga = await Mangas.CreateFromWeb(new Uri(mangaInfo.Uri)).ConfigureAwait(false);
        sw.Start();
        DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
        await manga.Download().ConfigureAwait(false);
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
        if (mangaInfo.Uri == MangaInfos.Mangachan.EveScramble.Uri)
        {
          var login = context.Get<MangachanLogin>().Single();
          login.PasswordHash = "e84fce6c43aacd7f8452409a63083c18";
          login.UserId = "282433";
          login.IsLogined = true;
          await context.Save(login).ConfigureAwait(false);
        }

        var mangaUri = new Uri(mangaInfo.Uri);
        var existsManga = context.Get<IManga>().FirstOrDefault(m => m.Uri == mangaUri);
        if (existsManga != null)
          await context.Delete(existsManga).ConfigureAwait(false);
        manga = await Mangas.CreateFromWeb(mangaUri).ConfigureAwait(false);
      }

      Assert.AreEqual(mangaInfo.Description, manga.Description);
      if (Equals(mangaInfo.Status, manga.Status))
        Assert.Pass();
      else
      {
        var storedWords = GetWords(mangaInfo.Status);
        var downloaded = GetWords(manga.Status);
        var changes = storedWords.Except(downloaded).Count() + downloaded.Except(storedWords).Count();
        // Status can contain regular chagned info, try to compare
        Assert.LessOrEqual(changes, 2);
      }
    }

    [Test, Ignore("Only for generate actual manga info (and check diff)")]
    public async Task GenerateInfoForTests()
    {
      var fields = typeof(MangaInfos).GetNestedTypes().SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public));
      var cacheAttributes = fields.Select(f => f.GetCustomAttribute<InfoCacheAttribute>()).ToList();
      var tasks = cacheAttributes.Select(u => Builder.Generate(u)).ToArray();
      await Task.WhenAll(tasks).ConfigureAwait(false);
      var infos = tasks.Select(t => t.Result).OrderBy(i => i.Uri).ToList();
      var json = JsonConvert.SerializeObject(infos);
      File.WriteAllText(Environment.MangaCache, json);
    }

    public static List<string> GetWords(string str)
    {
      return str
        .Split(new[] {" ", ",", System.Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => Regex.Replace(s, "\\d+(,\\d+)*", "some_counts", RegexOptions.None))
        .ToList();
    }
  }
}