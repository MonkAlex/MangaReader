using System;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate;

namespace MangaReader.CLI
{
  public static class Initialize
  {
    public static void Run()
    {
      Client.Init();
      Client.Start(new ConsoleProgress());

      var m = Mangas.Create(new Uri("http://mintmanga.com/war_cry___artbook__berserk"));
      m.Name = "213";
      m.AddHistory(new Uri("http://google.com"));
      m.Save();

      foreach (var manga in Repository.Get<IManga>())
      {
        Console.WriteLine("{0}:{1}", manga.Id, manga);
        var isInit = NHibernateUtil.IsPropertyInitialized(manga, nameof(manga.Histories));
        manga.Download();
      }
      Core.Services.Config.ConfigStorage.Instance.Save();
    }
  }
}