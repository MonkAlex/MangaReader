using System;
using System.Linq;
using MangaReader.Manga;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;

namespace MangaReader.Tests
{
  public static class Builder
  {
    private static readonly Uri Url = new Uri("http://example.com");

    public static Readmanga CreateReadmanga()
    {
      var manga = new Readmanga
      {
        Uri = Url,
        Status = "example status",
        NeedUpdate = false,
        Name = "readmanga from example"
      };
      manga.Save();
      return manga;
    }

    public static void DeleteReadmanga(Readmanga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static Acomics CreateAcomics()
    {
      var manga = new Acomics
      {
        Uri = Url,
        Status = "example status",
        NeedUpdate = false,
        Name = "Acomics from example"
      };
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
      var history = new MangaHistory()
      {
        Date = DateTime.Today,
        Uri = Url
      };
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
