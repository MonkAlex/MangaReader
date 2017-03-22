using System;
using System.Linq;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Manga
{
  [TestClass]
  public class MangachanStructure
  {
    private MangaReader.Core.ISiteParser parser = new Hentaichan.Mangachan.Parser();

    [TestMethod]
    public void AddMangachanMultiPages()
    {
      var manga = GetManga("http://mangachan.me/manga/3828-12-prince.html");
      Assert.AreEqual(16, manga.Volumes.Count);
      Assert.AreEqual(78, manga.Volumes.Sum(v => v.Chapters.Count));
    }

    [TestMethod]
    public void AddMangachanSingleChapter()
    {
      var manga = GetManga("http://mangachan.me/manga/20138-16000-honesty.html");
      Assert.AreEqual(1, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Volumes.Sum(v => v.Chapters.Count));
    }

    private Hentaichan.Mangachan.Mangachan GetManga(string url)
    {
      var manga = Mangas.CreateFromWeb(new Uri(url)) as Hentaichan.Mangachan.Mangachan;
      return manga;
    }
  }
}
