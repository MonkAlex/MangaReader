using System;
using System.Collections.Generic;
using System.Linq;
using Acomics;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Acomics : TestClass
  {
    private Parser parser = new Parser();
    
    [Test]
    public void SearchOnAcomics()
    {
      var mangas = Search("mass effect");
      Assert.IsTrue(mangas.Any(m => m.Uri.OriginalString == "https://acomics.ru/~mess-perfect-3"));
    }

    private IEnumerable<IManga> Search(string name)
    {
      return parser.Search(name);
    }

  }
}