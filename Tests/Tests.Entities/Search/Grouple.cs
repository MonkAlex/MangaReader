using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Grouple : TestClass
  {
    [Test]
    public void SearchOnGrouple()
    {
      var manga = Search("Дурень и Сиськи").FirstOrDefault();
      Assert.AreEqual("Baka And Boing", manga.Name);
    }

    private IEnumerable<IManga> Search(string name)
    {
      return new global::Grouple.Parser().Search(name);
    }

  }
}