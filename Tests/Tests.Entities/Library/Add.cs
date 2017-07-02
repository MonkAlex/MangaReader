using System;
using System.Linq;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Library
{
  [TestFixture]
  public class Add : TestClass
  {
    protected LibraryViewModel model = new MangaReader.Core.Services.LibraryViewModel();

    [Test]
    public void AddInvalidUrl()
    {
      var error = false;
      var result = false;
      try
      {
        result = model.Add(@"http://example.com/");
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsFalse(result);
    }

    [Test]
    public void AddValidUrl()
    {
      var error = false;
      var result = false;
      var uri = new Uri(@"http://readmanga.me/berserk");

      var mangas = MangaReader.Core.NHibernate.Repository.Get<Grouple.Readmanga>()
        .ToList()
        .Where(m => m.Uri.AbsoluteUri.Contains("berserk"))
        .ToList();
      foreach (var manga in mangas)
        model.Remove(manga);

      try
      {
        result = model.Add(uri);
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsTrue(result);

      // Проверка повторного добавления.
      result = model.Add(uri);
      Assert.IsFalse(result);
    }
  }
}
