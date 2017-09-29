using System;
using System.Collections.Generic;
using System.Linq;
using Hentaichan;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.Search
{
  [TestFixture]
  public class Henchan : TestClass
  {
    private Parser parser = new Parser();
    
    [Test]
    public void SearchOnHenchan()
    {
      var mangas = Search("play");
      Assert.IsTrue(mangas.Any(m => m.Uri.OriginalString == "http://hentai-chan.me/manga/22839-shalosti-v-basseyne.html"));

      var game = Search("A Tale of Two Swords ");
      Assert.IsEmpty(game);
    }

    private IEnumerable<IManga> Search(string name)
    {
      return parser.Search(name);
    }

  }
}