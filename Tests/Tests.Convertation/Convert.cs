using System.IO;
using System.Linq;
using MangaReader;
using MangaReader.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Tests.Convertation
{
  [TestClass]
  public class Convert
  {
    [TestMethod]
    [DeploymentItem(@"Tests\Tests.Convertation\1.12")]
    public void Convert_1_12_to_main()
    {
      File.Move("db", "history");
      File.Move("list.txt", "db");
      File.Delete("storage.db");
      foreach (var file in Directory.GetFiles(".", "*.dbak*", SearchOption.TopDirectoryOnly))
      {
        File.Delete(file);
      }

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      var loadedMangas = MangaReader.Mapping.Environment.Session.Query<Mangas>().Count();
      Assert.AreEqual(75, loadedMangas);

      var loadedHistoryRecord = MangaReader.Mapping.Environment.Session.Query<MangaHistory>().Count();
      Assert.AreEqual(3036, loadedHistoryRecord);
    }

    [TestMethod]
    [DeploymentItem(@"Tests\Tests.Convertation\1.20")]
    public void Convert_1_20_to_main()
    {
      File.Delete("storage.db");
      foreach (var file in Directory.GetFiles(".", "*.dbak*", SearchOption.TopDirectoryOnly))
      {
        File.Delete(file);
      }

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      var loadedMangas = MangaReader.Mapping.Environment.Session.Query<Mangas>().Count();
      Assert.AreEqual(159, loadedMangas);

      var loadedHistoryRecord = MangaReader.Mapping.Environment.Session.Query<MangaHistory>().Count();
      Assert.AreEqual(17857, loadedHistoryRecord);
    }
  }
}
