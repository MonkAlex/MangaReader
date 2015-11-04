using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace MangaReader.Tests.Convertation
{
  [TestClass]
  public class Convert
  {
    public void Deploy(string version)
    {
      var currentDirectory = Directory.GetCurrentDirectory();
      var directory = new DirectoryInfo(Path.Combine(currentDirectory, "..", "..", "Convertation", version));
      if (directory.Exists)
      {
        foreach (var file in directory.GetFiles())
        {
          file.CopyTo(Path.Combine(currentDirectory, file.Name), true);
        }
      }
    }

    [TestMethod]
    public void Convert_1_12_to_main()
    {
      Deploy("1.12");
      File.Move("db", "history");
      File.Move("list.txt", "db");

      Environment.TestCleanup();
      File.Delete("storage.db");
      foreach (var file in Directory.GetFiles(".", "*.dbak*", SearchOption.TopDirectoryOnly))
      {
        File.Delete(file);
      }
      Environment.TestInitialize(null);

      var loadedMangas = Mapping.Environment.Session.Query<Mangas>().Count();
      Assert.AreEqual(75, loadedMangas);

      var loadedHistoryRecord = Mapping.Environment.Session.Query<MangaHistory>().Count();
      Assert.AreEqual(5951, loadedHistoryRecord);
    }

    [TestMethod]
    public void Convert_1_20_to_main()
    {
      Deploy("1.20");
      Environment.TestCleanup();
      File.Delete("storage.db");
      foreach (var file in Directory.GetFiles(".", "*.dbak*", SearchOption.TopDirectoryOnly))
      {
        File.Delete(file);
      }
      Environment.TestInitialize(null);

      var loadedMangas = Mapping.Environment.Session.Query<Mangas>().Count();
      Assert.AreEqual(159, loadedMangas);

      var loadedHistoryRecord = Mapping.Environment.Session.Query<MangaHistory>().Count();
      Assert.AreEqual(17857, loadedHistoryRecord);
    }
  }
}
