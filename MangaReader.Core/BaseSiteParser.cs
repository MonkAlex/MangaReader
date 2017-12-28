using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core
{
  public abstract class BaseSiteParser : ISiteParser
  {
    protected static ConcurrentDictionary<Type, IMapper> Mappers = new ConcurrentDictionary<Type, IMapper>();

    public abstract void UpdateNameAndStatus(IManga manga);

    public virtual void UpdateContentType(IManga manga)
    {
      // Content type cannot be changed.
    }

    public abstract void UpdateContent(IManga manga);

    public abstract UriParseResult ParseUri(Uri uri);

    protected static void UpdateName(IManga manga, string newName)
    {
      if (string.IsNullOrWhiteSpace(newName))
        Log.AddFormat("Не удалось получить имя манги, текущее название - '{0}'.", manga.ServerName);
      else if (newName != manga.ServerName)
        manga.ServerName = newName;
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="chapters">Главы.</param>
    protected void FillMangaChapters(IManga manga, ICollection<ChapterDto> chapters)
    {
      if (!chapters.Any())
        return;

      GetMapper().Map(chapters, manga.Chapters);
      foreach (var chapter in manga.Chapters)
      {
        chapter.Manga = manga;
        foreach (var page in chapter.Container)
        {
          page.Chapter = chapter;
        }
      }
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы и тома.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="volumes">Тома.</param>
    /// <remarks>Перенос главы из тома в том видимо не ловим.</remarks>
    protected void FillMangaVolumes(IManga manga, ICollection<VolumeDto> volumes)
    {
      if (!volumes.Any())
        return;

      GetMapper().Map(volumes, manga.Volumes);
      foreach (var volume in manga.Volumes)
      {
        volume.Manga = manga;
        foreach (var chapter in volume.Container)
        {
          chapter.Volume = volume;
          foreach (var page in chapter.Container)
          {
            page.Chapter = chapter;
          }
        }
      }
    }

    /// <summary>
    /// Обновляем страницы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="pages">Страницы.</param>
    protected void FillMangaPages(IManga manga, ICollection<MangaPageDto> pages)
    {
      if (!pages.Any())
        return;

      GetMapper().Map(pages, manga.Pages);
      foreach (var page in manga.Pages)
      {
        page.Manga = manga;
      }
    }

    public abstract IEnumerable<byte[]> GetPreviews(IManga manga);

    public virtual IAsyncEnumerable<IManga> Search(string name)
    {
      var client = new CookieClient();
      var hosts = ConfigStorage.Plugins
        .Where(p => p.GetParser().GetType() == this.GetType())
        .Select(p => p.GetSettings().MainUri);
      return hosts.SelectAsync(async host => await GetMangaNodes(name, host, client))
        .Where(nc => nc != null)
        .SelectMany(n => n.Item1.SelectAsync(node => GetMangaFromNode(n.Item2, client, node)))
        .Where(m => m != null);
    }

    protected abstract Task<Tuple<HtmlNodeCollection, Uri>> GetMangaNodes(string name, Uri host, CookieClient client);

    protected abstract Task<IManga> GetMangaFromNode(Uri host, CookieClient client, HtmlNode manga);

    public virtual IMapper GetMapper()
    {
      return Mappers.GetOrAdd(typeof(BaseSiteParser), type =>
      {
        var config = new MapperConfiguration(cfg =>
        {
          cfg.AddCollectionMappers();
          cfg.CreateMap<VolumeDto, Volume>()
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, Chapter>()
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}