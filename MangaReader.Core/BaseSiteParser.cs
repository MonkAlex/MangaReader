using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

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
      GetMapper().Map(chapters, manga.Chapters);
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы и тома.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="volumes">Тома.</param>
    /// <remarks>Перенос главы из тома в том видимо не ловим.</remarks>
    protected void FillMangaVolumes(IManga manga, ICollection<VolumeDto> volumes)
    {
      GetMapper().Map(volumes, manga.Volumes);
    }

    /// <summary>
    /// Обновляем страницы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="pages">Страницы.</param>
    protected void FillMangaPages(IManga manga, ICollection<MangaPageDto> pages)
    {
      GetMapper().Map(pages, manga.Pages);
    }

    public abstract IEnumerable<byte[]> GetPreviews(IManga manga);

    public abstract IEnumerable<IManga> Search(string name);

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