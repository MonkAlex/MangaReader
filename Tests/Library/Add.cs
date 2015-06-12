using System;
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
        result = MangaReader.Services.Library.Add(@"http://example.com/");
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
      try
      {
        result = MangaReader.Services.Library.Add(@"http://readmanga.me/berserk/");
      }
      catch (Exception)
      {
        error = true;
      }
      Assert.IsFalse(error);
      Assert.IsTrue(result);

      // Проверка повторного добавления.
      result = MangaReader.Services.Library.Add(@"http://readmanga.me/berserk/");
      Assert.IsFalse(result);
    }
  }
}
