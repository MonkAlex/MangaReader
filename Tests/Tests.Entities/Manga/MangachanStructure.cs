using System;
using System.Linq;
using System.Threading.Tasks;
using Hentaichan.Mangachan;
using MangaReader.Core.Manga;
using NUnit.Framework;
using MangaReader.Core.Services.Config;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MangachanStructure : TestClass
  {
    [Test]
    public async Task AddMangachanMultiPages()
    {
      var manga = await GetManga(MangaInfos.Mangachan.ThisGirlfriendIsFiction.Uri).ConfigureAwait(false);
      await new Parser().UpdateContent(manga).ConfigureAwait(false);
      Assert.AreEqual(4, manga.Volumes.Count);
      Assert.AreEqual(34, manga.Volumes.Sum(v => v.Container.Count()));
    }

    [Test]
    public async Task AddMangachanSingleChapter()
    {
      var manga = await GetManga("https://manga-chan.me/manga/20138-16000-honesty.html").ConfigureAwait(false);
      await new Parser().UpdateContent(manga).ConfigureAwait(false);
      Assert.AreEqual(1, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Volumes.Sum(v => v.Container.Count()));
    }

    private static async Task<IManga> GetManga(string url)
    {
      return await Mangas.CreateFromWeb(new Uri(url)).ConfigureAwait(false);
    }

    [Test]
    [Parallelizable(ParallelScope.None)]
    public async Task MangachanNameParsing()
    {
      // Спецсимвол "
      await TestNameParsing("https://manga-chan.me/manga/48069-isekai-de-kuro-no-iyashi-te-tte-yobarete-imasu.html",
        "Isekai de \"Kuro no Iyashi Te\" tte Yobarete Imasu",
        "В другом мире моё имя - Чёрный целитель").ConfigureAwait(false);

      // Просто проверка.
      await TestNameParsing("https://manga-chan.me/manga/46475-shin5-kekkonshite-mo-koishiteru.html",
        "#shin5 - Kekkonshite mo Koishiteru",
        "Любовь после свадьбы").ConfigureAwait(false);

      // Нет русского варианта.
      await TestNameParsing("https://manga-chan.me/manga/17625--okazaki-mari.html",
        "& (Okazaki Mari)",
        "& (Okazaki Mari)").ConfigureAwait(false);

      // Символ звездочки *
      await TestNameParsing("https://manga-chan.me/manga/23099--asterisk.html",
        "* - Asterisk",
        "Звездочка").ConfigureAwait(false);
    }

    private async Task TestNameParsing(string uri, string english, string russian)
    {
      ConfigStorage.Instance.AppConfig.Language = Languages.English;
      var manga = await GetManga(uri).ConfigureAwait(false);
      Assert.AreEqual(english, manga.Name);
      ConfigStorage.Instance.AppConfig.Language = Languages.Russian;
      await new Parser().UpdateNameAndStatus(manga).ConfigureAwait(false);
      Assert.AreEqual(russian, manga.Name);
    }
  }
}
