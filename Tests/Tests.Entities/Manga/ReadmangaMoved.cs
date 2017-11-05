using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NHibernate.Linq;
using NUnit.Framework;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class ReadmangaMoved : TestClass
  {
    [Test]
    public void CreateWithHistoryAndMove()
    {
      var model = new MangaReader.Core.Services.LibraryViewModel();
      foreach (var remove in Repository.Get<IManga>().Where(m => m.ServerName.Contains("btooom")).ToList())
        model.Remove(remove);

      var manga = Builder.CreateReadmanga();
      manga.Uri = new Uri("http://readmanga.me/btoom");
      manga.Histories.Add(new MangaReader.Core.Manga.MangaHistory(new Uri("http://readmanga.me/btoom/vol1/1?mature=1")));
      manga.Save();

      manga = Repository.Get<Grouple.Readmanga>(manga.Id);
      manga.Refresh();
      manga.Save();


      var volume = new Volume();
      volume.Container.Add(new Chapter(new Uri("http://mintmanga.com/btooom_/vol1/1?mature=1")));

      var chartersNotInHistory = History.GetItemsWithoutHistory(volume);
      Assert.AreEqual(0, chartersNotInHistory.Count);
    }
  }
}
