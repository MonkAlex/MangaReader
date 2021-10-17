using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using Newtonsoft.Json;

namespace Tests.Entities
{
  public static class Builder
  {
    public static readonly Uri Url = new Uri("http://example.com");
    public static readonly Uri ReadmangaUri = new Uri("https://readmanga.io/adele");
    public static readonly Uri AcomicsUri = new Uri("https://acomics.ru/~doodle-time");
    private static List<MangaInfo> _mangaInfos;

    /// <summary>
    /// Создать мангу.
    /// </summary>
    /// <returns></returns>
    public static async Task<Grouple.Readmanga> CreateReadmanga()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await Mangas.Create(ReadmangaUri).ConfigureAwait(false) as Grouple.Readmanga;
        manga.Status = "example status";
        manga.NeedUpdate = false;
        manga.Name = "readmanga from example" + Guid.NewGuid();
        await context.Save(manga).ConfigureAwait(false);
        return manga;
      }
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static async Task DeleteReadmanga(Grouple.Readmanga manga)
    {
      if (manga == null)
        return;

      using (var context = Repository.GetEntityContext())
      {
        await context.Delete(manga).ConfigureAwait(false);
      }
    }

    public static async Task<Acomics.Acomics> CreateAcomics()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await Mangas.Create(AcomicsUri).ConfigureAwait(false) as Acomics.Acomics;
        manga.Status = "example status";
        manga.NeedUpdate = false;
        manga.Name = "Acomics from example" + Guid.NewGuid();
        await context.Save(manga).ConfigureAwait(false);
        return manga;
      }
    }

    public static async Task DeleteAcomics(Acomics.Acomics manga)
    {
      if (manga == null)
        return;

      using (var context = Repository.GetEntityContext())
      {
        await context.Delete(manga).ConfigureAwait(false);
      }
    }

    public static async Task CreateMangaHistory(IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        var history = new MangaReader.Core.Manga.MangaHistory(Url);
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(history.Uri));
        await context.Save(manga).ConfigureAwait(false);
      }
    }

    public static async Task DeleteMangaHistory(IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        manga.ClearHistory();
        await context.Save(manga).ConfigureAwait(false);
      }
    }

    public static async Task<MangaInfo> Generate(InfoCacheAttribute cacheAttribute)
    {
      var info = new MangaInfo();
      info.Uri = cacheAttribute.Uri;

      var manga = await Mangas.CreateFromWeb(new Uri(cacheAttribute.Uri)).ConfigureAwait(false);
      if (cacheAttribute.Downloadable)
      {
        await manga.Download().ConfigureAwait(false);

        var files = Directory.GetFiles(manga.GetAbsoluteFolderPath(), "*", SearchOption.AllDirectories);
        var fileInfos = files.Select(f => new FileInfo(f)).ToList();
        info.FilesInFolder = fileInfos.Count;
        info.FolderSize = fileInfos.Sum(f => f.Length);

        info.AllFilesUnique = 1 == fileInfos.GroupBy(f => f.Length).Max(g => g.Count());
      }

      info.Status = manga.Status;
      info.Description = manga.Description;

      return info;
    }

    public static MangaInfo LoadFromCache(InfoCacheAttribute cacheAttribute)
    {
      if (File.Exists(Environment.MangaCache))
      {
        if (_mangaInfos == null)
          _mangaInfos = JsonConvert.DeserializeObject<List<MangaInfo>>(File.ReadAllText(Environment.MangaCache));
        return _mangaInfos.FirstOrDefault(m => m.Uri == cacheAttribute.Uri);
      }

      return default(MangaInfo);
    }
  }
}
