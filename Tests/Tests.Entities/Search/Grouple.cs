using System;
using System.Collections.Generic;
using System.Linq;
using Grouple;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Grouple : TestClass
  {
    [Test]
    public void SearchOnMintmanga()
    {
      var manga = SearchOnMintmanga("Baka And Boing").FirstOrDefault();
      Assert.AreEqual("Дурень и Сиськи", manga.Name);
      Assert.IsAssignableFrom<Mintmanga>(manga);
    }

    [Test]
    public void SearchOnReadmanga()
    {
      var manga = SearchOnReadmanga("Baka And Boing").FirstOrDefault();
      Assert.AreEqual("Дурни, тесты, аватары", manga.Name);
      Assert.IsAssignableFrom<Readmanga>(manga);
    }

    private IEnumerable<IManga> SearchOnMintmanga(string name)
    {
      return new global::Grouple.MintmangaParser().Search(name).ToEnumerable();
    }

    private IEnumerable<IManga> SearchOnReadmanga(string name)
    {
      return new global::Grouple.ReadmangaParser().Search(name).ToEnumerable();
    }
  }
}
