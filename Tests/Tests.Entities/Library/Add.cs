using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Library
{
  [TestClass]
  public class Add
  {
    [TestMethod]
    public void AddInvalidUrl()
    {
      var error = false;
      var result = false;
      try
      {
        result = MangaReader.Core.Services.Library.Add(@"http://example.com/");
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
        MangaReader.Core.Services.Library.Remove(manga);

      try
      {
        result = MangaReader.Core.Services.Library.Add(uri);
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsTrue(result);

      // Проверка повторного добавления.
      result = MangaReader.Core.Services.Library.Add(uri);
      Assert.IsFalse(result);
    }
  }
}
