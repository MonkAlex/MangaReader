using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.PropertyParsing
{
  [TestFixture]
  public class Description : TestClass
  {
    [Test]
    public void CheckAcomics()
    {
      var manga = CreateMangaIgnoreError("https://acomics.ru/~supersciencefriends");
      Assert.AreEqual("Комикс \"\r\nДрузья Супер Ученые\r\n: 2099\" является дополнением к анимационному сериалу\r\n\"Друзья Супер Ученые\"\r\n.", manga.Description);
      Assert.IsTrue(manga.Status.Contains("https://acomics.ru/~supersciencefriends\r\nАвтор оригинала: Tinman Creative Studios\r\nАвтор оригинала: Tinman Creative Studios\r\nПереводчик: CrownedPenguin\r\nКоличество выпусков: 44\r\nКоличество подписчиков:"));
      Assert.IsTrue(manga.Status.Contains("\r\nОфициальный сайт: http://supersciencefriends.com/2099/\r\nВозрастной рейтинг: Parents strongly cautioned (Не рекомендуется лицам до 13 лет)\r\nЛицензия: Нет лицензии или не CC\r\n"));
    }

    [Test]
    public void CheckReadmanga()
    {
      var manga = CreateMangaIgnoreError("http://readmanga.me/kuroshitsuji_dj___black_sheep");
      Assert.AreEqual("Все знают безупречного дворецкого поместья Фантомхайв. Но подождите...в этой истории что-то не так... что здесь забыла овца?!", manga.Description);
      Assert.AreEqual("Томов: 1\r\nПеревод: завершен\r\nЖанр: додзинси\r\nАвтор: Kiyo\r\nВозрастная рекомендация: PG-13\r\nПереводчик: Manga-Kya\r\n", manga.Status);
    }

    [Test]
    public void CheckHentaichan()
    {
      var manga = CreateMangaIgnoreError("http://henchan.me/manga/14212-love-and-devil-glava-25.html");
      Assert.AreEqual("Конец серии + бонусная история", manga.Description);
      Assert.AreEqual("Серия Оригинальные работы\r\nАвтор ﻿Yanagi Masashi\r\nПереводчик ﻿Илион\r\nЯзык На русском\r\n", manga.Status);
    }

    [Test]
    public void CheckMintmanga()
    {
      var manga = CreateMangaIgnoreError("http://readmanga.me/love_mate_2");
      Assert.AreEqual("4-кадровая манга дополнительно включенная в тома манги \"Город,в котором ты живешь\"", manga.Description);
      Assert.IsTrue(manga.Status.Contains("Томов: 1\r\nПеревод: завершен\r\n"));
      Assert.IsTrue(manga.Status.Contains("Категория: Ёнкома\r\nАвтор: Сэо Кодзи\r\nГод выпуска: 2008\r\nИздательство: Kodansha\r\nПереводчик: Aoshi Shinomori\r\n"));
    }

    [Test]
    public void CheckMangachan()
    {
      var manga = CreateMangaIgnoreError("http://mangachan.me/manga/15659-this-girlfriend-is-fiction.html");
      Assert.AreEqual("Наш главный герой - наивный пацан, по уши влюблённый в девушку постарше чем он, которую все считают гениальным писателем. Он настолько влюбился в неё, что забыл о своем секретном хобби. Секрет заключался в том, что последние 10 лет он занимался составлением \"девушки своей мечты\". Её день рождения, индивидуальность, хобби, разговорный тон и так далее. Это всё было записано в огромной стопке бумаг. Всё это время его воображение было для него и другом, и возлюбленной, но теперь он влюблён в совсем живую и даже реальную девушку. Сделав свой выбор, он сжигает весь свой многолетний труд. К его удивлению, она была рядом. Начался дождь и в процессе беседы, она призналась, что имеет трудности с написанием своего романа из-за отсутствия опыта. С усилением шторма, наш пацанчик, заполучив информацию о её слабости, решается открыть ей свой секрет. Собрав волю в кулак, он открывает ящик, где были собраны все его записи... помимо этого в ящике была обнажённая девушка, которую он создавал на протяжении 10 лет!", manga.Description);
      Assert.AreEqual("Другие названия Kono Kanojo wa Fiction desu\r\nТип Манга\r\nАвтор ﻿Watanabe Shizumu\r\nСтатус (Томов) 4 тома, выпуск завершен\r\nЗагружено 34 глав, перевод завершен\r\nТэги эротика, сёнэн, школа, романтика, комедия, сверхъестественное\r\nПереводчики ﻿Ready Steady GO! (R.S.G.), Syndicate Manga Team\r\n", manga.Status);
    }

    [Test]
    public void CheckMangachanWithHtml()
    {
      var manga = CreateMangaIgnoreError("http://mangachan.me/manga/64187-eve-scramble.html");
      Assert.AreEqual("Есть девушка, способная осчастливить жизнь любого, даже проведя с ним всего лишь один день. Эта девушка, по имени Ева, когда её похитил неизвестный, присылает мне сообщение с просьбой о помощи. Однако, похоже, не я один был готов, бросив всё, бежать к ней на выручку. В действие вступает грандиозный план по вызволению Евы, разработанный особым альянсом бывших парней , который был создан с целью спасти любимую девушку!", manga.Description);
      Assert.AreEqual("Другие названия\r\nТип Манхва\r\nАвтор ﻿Hale, Gaonbi\r\nСтатус (Томов) 1 том, выпуск продолжается\r\nЗагружено 5 глав, перевод продолжается\r\nТэги драма, мистика, романтика, ужасы, веб\r\nПереводчики ﻿Disolver el lirio, Черный розариум\r\n", manga.Status);
    }

    protected IManga CreateMangaIgnoreError(string uri)
    {
      using (var context = Repository.GetEntityContext(nameof(Description)))
      {
        var mangaUri = new Uri(uri);
        var manga = context.Get<IManga>().FirstOrDefault(m => m.Uri == mangaUri);
        if (manga != null)
          context.Delete(manga);
        return Mangas.CreateFromWeb(mangaUri);
      }
    }
  }
}