using System;
using System.Linq;
using System.Threading.Tasks;
using Grouple;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Library
{
  [TestFixture]
  public class Add : TestClass
  {
    protected LibraryViewModel model = new MangaReader.Core.Services.LibraryViewModel();

    [Test]
    public async Task AddInvalidUrl()
    {
      var error = false;
      var result = true;
      IManga manga = null;
      try
      {
        (result, manga) = await model.Add(new Uri(@"http://example.com/")).ConfigureAwait(false);
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsFalse(result);
      Assert.IsNull(manga);
    }

    [Test]
    public async Task AddValidUrl()
    {
      var error = false;
      var result = false;
      var uri = new Uri(@"https://mintmanga.live/berserk");

      using (var context = Repository.GetEntityContext())
      {
        var mangas = (await context.Get<Readmanga>()
            .ToListAsync().ConfigureAwait(false))
          .Where(m => m.Uri.AbsoluteUri.Contains("berserk"))
          .ToList();
        foreach (var manga in mangas)
          await model.Remove(manga).ConfigureAwait(false);
      }

      try
      {
        result = (await model.Add(uri).ConfigureAwait(false)).Success;
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsTrue(result);

      // Проверка повторного добавления.
      result = (await model.Add(uri).ConfigureAwait(false)).Success;
      Assert.IsFalse(result);
    }
  }
}
