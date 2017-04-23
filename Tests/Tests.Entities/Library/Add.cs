using System;
using System.Linq;
using MangaReader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Library
{
  [TestClass]
  public class Add
  {
    protected LibraryViewModel model = new MangaReader.Core.Services.LibraryViewModel();

    [TestMethod]
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

    [TestMethod]
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
