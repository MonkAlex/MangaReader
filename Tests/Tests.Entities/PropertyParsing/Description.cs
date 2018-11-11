using System;
using MangaReader.Core.Manga;
using NUnit.Framework;

namespace Tests.Entities.PropertyParsing
{
  [TestFixture]
  public class Description : TestClass
  {
    [Test]
    public void CheckAcomics()
    {
      var manga = Mangas.CreateFromWeb(new Uri("https://acomics.ru/~supersciencefriends"));
      Assert.AreEqual("Комикс \"\r\nДрузья Супер Ученые\r\n: 2099\" является дополнением к анимационному сериалу\r\n\"Друзья Супер Ученые\"\r\n.", manga.Description);
      Assert.AreEqual("https://acomics.ru/~supersciencefriends\r\nАвтор оригинала: Tinman Creative Studios\r\nАвтор оригинала: Tinman Creative Studios\r\nПереводчик: CrownedPenguin\r\nКоличество выпусков: 44\r\nКоличество подписчиков: 281\r\nОфициальный сайт: http://supersciencefriends.com/2099/\r\nВозрастной рейтинг: Parents strongly cautioned (Не рекомендуется лицам до 13 лет)\r\nЛицензия: Нет лицензии или не CC\r\n", manga.Status);
      
    }

    [Test]
    public void CheckReadmanga()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://readmanga.me/kuroshitsuji_dj___black_sheep"));
      Assert.AreEqual("Все знают безупречного дворецкого поместья Фантомхайв. Но подождите...в этой истории что-то не так... что здесь забыла овца?!", manga.Description);
      Assert.AreEqual("Томов: 1\r\nПеревод: завершен\r\nЖанр: додзинси\r\nАвтор: Kiyo\r\nВозрастная рекомендация: PG-13\r\nПереводчик: Manga-Kya\r\n", manga.Status);
    }

    [Test]
    public void CheckHentaichan()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://hentai-chan.me/manga/14212-love-and-devil-glava-25.html"));
      Assert.AreEqual("Конец серии + бонусная история", manga.Description);
      Assert.AreEqual("Серия Оригинальные работы\r\nАвтор ﻿Yanagi Masashi\r\nПереводчик ﻿Илион\r\nЯзык На русском\r\n", manga.Status);
    }

    [Test]
    public void CheckMintmanga()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://mintmanga.com/love_mate"));
      Assert.AreEqual("Ямато, приехал учиться в высшей школе в Токио из предместья Хиросимы. По пути к своей тёте, у которой он поселился на время учёбы,  встречает красивую девушку и влюбляется в нее с первого взгляда. Позже оказывается, что девушка — его соседка, зовут её Судзука, и она  поступила в ту же высшую школу. Он пытается привлечь её внимание, и даже вступает в команду по лёгкой атлетике, в которой она состоит.", manga.Description);
      Assert.AreEqual("Томов: 18\r\nПеревод: завершен\r\nЖанры: гарем, этти, повседневность, школа, драма, сёнэн, романтика, спорт …\r\nКатегория: Не Яой\r\nАвтор: Сэо Кодзи\r\nГод выпуска: 2003\r\nИздательство: Kodansha\r\nЖурнал: Shuukan Shounen Magazine\r\nПереводчики: Ной Студия, AngelVan, cheer_less\r\n", manga.Status);
    }

    [Test]
    public void CheckMangachan()
    {
      var manga = Mangas.CreateFromWeb(new Uri("http://mangachan.me/manga/15659-this-girlfriend-is-fiction.html"));
      Assert.AreEqual("Наш главный герой - наивный пацан, по уши влюблённый в девушку постарше чем он, которую все считают гениальным писателем. Он настолько влюбился в неё, что забыл о своем секретном хобби. Секрет заключался в том, что последние 10 лет он занимался составлением \"девушки своей мечты\". Её день рождения, индивидуальность, хобби, разговорный тон и так далее. Это всё было записано в огромной стопке бумаг. Всё это время его воображение было для него и другом, и возлюбленной, но теперь он влюблён в совсем живую и даже реальную девушку. Сделав свой выбор, он сжигает весь свой многолетний труд. К его удивлению, она была рядом. Начался дождь и в процессе беседы, она призналась, что имеет трудности с написанием своего романа из-за отсутствия опыта. С усилением шторма, наш пацанчик, заполучив информацию о её слабости, решается открыть ей свой секрет. Собрав волю в кулак, он открывает ящик, где были собраны все его записи... помимо этого в ящике была обнажённая девушка, которую он создавал на протяжении 10 лет!", manga.Description);
      Assert.AreEqual("Другие названия Kono Kanojo wa Fiction desu\r\nТип Манга\r\nАвтор ﻿Watanabe Shizumu\r\nСтатус (Томов) 4 тома, выпуск завершен\r\nЗагружено 34 глав, перевод завершен\r\nТэги эротика, сёнэн, школа, романтика, комедия, сверхъестественное\r\nПереводчики ﻿Ready Steady GO! (R.S.G.), Syndicate Manga Team\r\n", manga.Status);
    }

  }
}