using System;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace Tests.Entities
{
  public static class Builder
  {
    public static readonly Uri Url = new Uri("http://example.com");
    public static readonly Uri ReadmangaUri = new Uri("http://readmanga.me/adele");
    public static readonly Uri AcomicsUri = new Uri("https://acomics.ru/~doodle-time");

    /// <summary>
    /// Создать мангу.
    /// </summary>
    /// <returns></returns>
    public static Grouple.Readmanga CreateReadmanga()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Mangas.Create(ReadmangaUri) as Grouple.Readmanga;
        manga.Status = "example status";
        manga.NeedUpdate = false;
        manga.Name = "readmanga from example" + Guid.NewGuid();
        context.Save(manga);
        return manga;
      }
    }

    /// <summary>
    /// Удалить мангу.
    /// </summary>
    /// <param name="manga"></param>
    public static void DeleteReadmanga(Grouple.Readmanga manga)
    {
      if (manga == null)
        return;

      using (var context = Repository.GetEntityContext())
      {
        context.Delete(manga);
      }
    }

    public static Acomics.Acomics CreateAcomics()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Mangas.Create(AcomicsUri) as Acomics.Acomics;
        manga.Status = "example status";
        manga.NeedUpdate = false;
        manga.Name = "Acomics from example" + Guid.NewGuid();
        context.Save(manga);
        return manga;
      }
    }

    public static void DeleteAcomics(Acomics.Acomics manga)
    {
      if (manga == null)
        return;

      using (var context = Repository.GetEntityContext())
      {
        context.Delete(manga);
      }
    }

    public static void CreateMangaHistory(IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        var history = new MangaReader.Core.Manga.MangaHistory(Url);
        manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(history.Uri));
        context.Save(manga);
      }
    }

    public static void DeleteMangaHistory(IManga manga)
    {
      using (var context = Repository.GetEntityContext())
      {
        manga.ClearHistory();
        context.Save(manga);
      }
    }
  }
}
