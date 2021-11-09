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
    private static IEnumerable<MangaInfo> MangaToDownload()
    {
      MangaInfos.Init();
      yield return MangaInfos.Acomics.MgsLdioh;
      yield return MangaInfos.Henchan.TwistedIntent;
      yield return MangaInfos.Hentai2Read.AttentionPlease;
      yield return MangaInfos.Mangachan.Rain;
      if (!AppveyorHelper.IsRunning())
        yield return MangaInfos.Mintmanga.ChiaChia;
      if (!AppveyorHelper.IsRunning())
        yield return MangaInfos.Readmanga.Kuroshitsuji;
    }

    private static IEnumerable<TestCaseData> MangaToValidateStatusAndDescription()
    {
      MangaInfos.Init();
      yield return new TestCaseData(MangaInfos.Acomics.SuperScienceFriends);
      yield return new TestCaseData(MangaInfos.Henchan.LoveAndDevil);
      yield return new TestCaseData(MangaInfos.Hentai2Read.AttentionPlease);
      yield return new TestCaseData(MangaInfos.Mangachan.ThisGirlfriendIsFiction);
      yield return new TestCaseData(MangaInfos.Mangachan.EveScramble).SetDescription("html codes in description");
      if (!AppveyorHelper.IsRunning())
        yield return new TestCaseData(MangaInfos.Mintmanga.LoveMate);
      if (!AppveyorHelper.IsRunning())
        yield return new TestCaseData(MangaInfos.Readmanga.Kuroshitsuji);
    }

    [Test, TestCaseSource(nameof(MangaToDownload))]
    public async Task Download(MangaInfo mangaInfo)
    {
      IManga manga;
      var sw = new Stopwatch();
      using (var context = Repository.GetEntityContext($"Test to download {mangaInfo.Uri}"))
      {
        foreach (var toDelete in context.Get<IManga>().Where(m => m.Uri.ToString() == mangaInfo.Uri))
        {
          await context.Delete(toDelete).ConfigureAwait(false);
          DirectoryHelpers.DeleteDirectory(toDelete.GetAbsoluteFolderPath());
        }

        manga = await Mangas.CreateFromWeb(new Uri(mangaInfo.Uri)).ConfigureAwait(false);
        DirectoryHelpers.DeleteDirectory(manga.GetAbsoluteFolderPath());
        sw.Start();
        await manga.Download().ConfigureAwait(false);
      }

      sw.Stop();
      Log.Add($"manga loaded {sw.Elapsed.TotalSeconds}, iscompleted = {manga.IsDownloaded}, downloaded = {manga.Downloaded}%");
      Assert.IsTrue(Directory.Exists(manga.GetAbsoluteFolderPath()));
      var files = Directory.GetFiles(manga.GetAbsoluteFolderPath(), "*", SearchOption.AllDirectories);
      Assert.AreEqual(mangaInfo.FilesInFolder, files.Length);
      var fileInfos = files.Select(f => new FileInfo(f)).ToList();
      Assert.AreEqual(mangaInfo.FolderSize, fileInfos.Sum(f => f.Length), mangaInfo.FolderSize/50.0);
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
        var existsManga = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Uri == mangaUri).ConfigureAwait(false);
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
        // Status can contain regular changed info, try to compare
        Assert.LessOrEqual(changes, 2);
      }
    }

    [Test, Ignore("Only for generate actual manga info (and check diff)")]
    public async Task GenerateInfoForTests()
    {
      var fields = typeof(MangaInfos).GetNestedTypes().SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public));
      var cacheAttributes = fields.Select(f => f.GetCustomAttribute<InfoCacheAttribute>()).ToList();
      var tasks = cacheAttributes.Select(u => Builder.Generate(u));
      List<MangaInfo> infos = new List<MangaInfo>();
      var completedTasks = tasks.Select(t => t.ContinueWith(task =>
      {
        if (task.Exception != null)
          return;

        infos.Add(task.Result);
      })).ToArray();
      await Task.WhenAll(completedTasks).ConfigureAwait(false);
      infos = infos.OrderBy(i => i.Uri).ToList();
      var json = JsonConvert.SerializeObject(infos);
      File.WriteAllText(Environment.MangaCache, json);
    }

    public static List<string> GetWords(string str)
    {
      return str
        .Split(new[] { " ", ",", System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => Regex.Replace(s, "\\d+(,\\d+)*", "some_counts", RegexOptions.None))
        .ToList();
    }
  }
}
