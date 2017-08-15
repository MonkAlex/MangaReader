using System;
using System.Collections.Generic;
using System.Linq;
using Hentaichan.Mangachan;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Mangachan : TestClass
  {
    private Parser parser = new Parser();
    
    [Test]
    public void SearchOnMangachan()
    {
      var mangas = Search("mass effect");
      Assert.IsTrue(mangas.Any(m => m.Uri.OriginalString == "http://mangachan.me/manga/62864-mass-effect-dj-romance.html"));
    }

    private IEnumerable<IManga> Search(string name)
    {
      return parser.Search(name);
    }

  }
}