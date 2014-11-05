using System;
using MangaReader.Manga.Acomic;
using MangaReader.Manga.Grouple;

namespace MangaReader.Tests
{
  public static class Builder
  {
    private const string Url = "http:\\example.com";

    public static Readmanga CreateReadmanga()
    {
      var manga = new Readmanga
      {
        Url = Url,
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
        Url = Url,
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

    public static MangaHistory CreateMangaHistory()
    {
      var manga = CreateAcomics();
      var history = new MangaHistory()
      {
        Date = DateTime.Today,
        Manga = manga,
        Url = Url
      };
      history.Save();
      return history;
    }

    public static void DeleteMangaHistory(MangaHistory history)
    {
      var manga = history.Manga;
      history.Delete();
      manga.Delete();
    }
  }
}
