using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Hentai2Read : TestClass
  {
    [Test]
    public void SearchOnHentai2Read()
    {
      var mangas = Search("Mushroom Bamboo Shoot");
      Assert.IsTrue(mangas.Any(m => m.Uri.OriginalString == "https://hentai2read.com/mushroom_bamboo_shoot/"));
      
      mangas = Search("A Wife’s Welcoming Intercourse");
      Assert.IsTrue(mangas.Any(m => m.Uri.OriginalString == "https://hentai2read.com/a_wifes_welcoming_intercourse/"));
    }

    private List<IManga> Search(string name)
    {
      return new global::Hentai2Read.com.Hentai2ReadParser().Search(name).ToEnumerable().ToList();
    }

  }
}
