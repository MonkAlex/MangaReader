using System.Linq;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Tests.Convertation._1._20
{
  [TestClass]
  public class Convert
  {
    [TestMethod]
    public void Convert_1_20_to_main()
    {
      Environment.Deploy(@"Tests.Convertation\1.20");

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      var loadedMangas = MangaReader.Core.NHibernate.Mapping.Session.Query<Mangas>().Count();
      Assert.AreEqual(159, loadedMangas);

      var loadedHistoryRecord = MangaReader.Core.NHibernate.Mapping.Session.Query<MangaHistory>().Count();
      Assert.AreEqual(17857, loadedHistoryRecord);
    }

    [TestInitialize]
    public void Clean()
    {
      Environment.BeforeTestClean();
    }

    [TestCleanup]
    public void Close()
    {
      Environment.TestCleanup();
    }
  }
}
