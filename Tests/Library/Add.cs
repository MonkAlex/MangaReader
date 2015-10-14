using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaReader.Tests.Library
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
        result = Services.Library.Add(@"http://example.com/");
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
      var uri = new Uri(@"http://readmanga.me/berserk/");

      var mangas = Services.Library.LibraryMangas.Where(m => Equals(m.Uri, uri)).ToList();
      foreach (var manga in mangas)
        Services.Library.Remove(manga);

      try
      {
        result = Services.Library.Add(uri);
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsTrue(result);

      // Проверка повторного добавления.
      result = Services.Library.Add(uri);
      Assert.IsFalse(result);
    }
  }
}
