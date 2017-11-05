using System;
using System.Linq;
using MangaReader.Core.Manga;

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
    public static Grouple.Readmanga CreateReadmanga()
    {
      var manga = Mangas.Create(ReadmangaUri) as Grouple.Readmanga;
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
    public static void DeleteReadmanga(Grouple.Readmanga manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static Acomics.Acomics CreateAcomics()
    {
      var manga = Mangas.Create(AcomicsUri) as Acomics.Acomics;
      manga.Status = "example status";
      manga.NeedUpdate = false;
      manga.Name = "Acomics from example" + Guid.NewGuid();
      manga.Save();
      return manga;
    }

    public static void DeleteAcomics(Acomics.Acomics manga)
    {
      if (manga == null)
        return;

      manga.Delete();
    }

    public static void CreateMangaHistory(IManga manga)
    {
      var history = new MangaReader.Core.Manga.MangaHistory(Url);
      manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(history.Uri));
      manga.Save();
    }

    public static void DeleteMangaHistory(IManga manga)
    {
      manga.Histories.Clear();
      manga.Save();
    }
  }
}
