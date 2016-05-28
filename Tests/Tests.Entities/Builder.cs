using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Manga.Acomic;
using MangaReader.Core.Manga.Grouple;

namespace Tests.Entities
{
  public static class Builder
  {
    private static readonly Uri Url = new Uri("http://example.com");
    private static readonly Uri ReadmangaUri = new Uri("http://readmanga.me");
    private static readonly Uri AcomicsUri = new Uri("http://acomics.ru");

    /// <summary>
    /// Создать мангу.
    /// </summary>
    /// <returns></returns>
    public static Readmanga CreateReadmanga()
    {
      var manga = Mangas.Create(ReadmangaUri) as Readmanga;
      manga.Status = "example status";
      manga.NeedUpdate = false;
      manga.Name = "readmanga from example" + Guid.NewGuid();
      manga.Save();
      return manga;
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static void DeleteReadmanga(Readmanga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static Acomics CreateAcomics()
    {
      var manga = Mangas.Create(AcomicsUri) as Acomics;
      manga.Status = "example status";
      manga.NeedUpdate = false;
      manga.Name = "Acomics from example" + Guid.NewGuid();
      manga.Save();
      return manga;
    }

    public static void DeleteAcomics(Acomics manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static void CreateMangaHistory(Mangas manga)
    {
      var history = new MangaHistory(Url);
      manga.Histories.Add(history);
      manga.Save();
    }

    public static void DeleteMangaHistory(Mangas manga)
    {
      var history = manga.Histories.ToList();
      foreach (var mangaHistory in history)
      {
        manga.Histories.Remove(mangaHistory);
      }
      manga.Save();
    }
  }
}
