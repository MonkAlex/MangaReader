using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.EquivalencyExpression;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace MangaReader.Core
{
  public abstract class BaseSiteParser : ISiteParser
  {
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
    protected static void FillMangaChapters(IManga manga, ICollection<ChapterDto> chapters)
    {
      AutoMapper.Mapper.Map(chapters, manga.Chapters);
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы и тома.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="volumes">Тома.</param>
    /// <remarks>Перенос главы из тома в том видимо не ловим.</remarks>
    protected static void FillMangaVolumes(IManga manga, ICollection<VolumeDto> volumes)
    {
      AutoMapper.Mapper.Map(volumes, manga.Volumes);
    }

    protected static void FillMangaPages(IManga manga, ICollection<MangaPageDto> pages)
    {
      AutoMapper.Mapper.Map(pages, manga.Pages);
    }

    public abstract IEnumerable<byte[]> GetPreviews(IManga manga);

    public abstract IEnumerable<IManga> Search(string name);

    internal static void InitMapper()
    {
      AutoMapper.Mapper.Initialize(cfg =>
      {
        cfg.AddCollectionMappers();
        cfg.CreateMap<VolumeDto, Volume>()
          .EqualityComparison((src, dest) => src.Number == dest.Number);
        cfg.CreateMap<ChapterDto, Chapter>()
          .EqualityComparison((src, dest) => src.Number == dest.Number);
        cfg.CreateMap<MangaPageDto, MangaPage>()
          .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
      });
    }
  }
}