using System.Collections.Generic;
using System.IO;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace Tests.Convertation._1._20
{
  [TestFixture]
  public class Convert
  {
    [Test]
    public void Convert_1_20_to_main()
    {
      Environment.DeployToRoot(Path.Combine("Tests.Convertation", "1.20"));

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      using (var context = Repository.GetEntityContext())
      {
        var loadedMangas = context.Get<IManga>().ToList();
        Assert.AreEqual(25, loadedMangas.Count);

        var loadedHistoryRecord = context.Get<MangaHistory>().ToList();

        /*
        var s = loadedMangas.Select(m => $"Assert.AreEqual({m.Histories.Count()}, loadedHistoryRecord.Count(h => h.Url.Contains(\"{m.Uri.OriginalString.Split('/').Last()}\")));").ToList();
        s.Add($"Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());");
        System.IO.File.WriteAllLines(@"D:\test.code", s);
        */

        Assert.AreEqual(557, loadedHistoryRecord.Count(h => h.Url.Contains("~flakypastry")));
        Assert.AreEqual(344, loadedHistoryRecord.Count(h => h.Url.Contains("~inferno")));
        Assert.AreEqual(676, loadedHistoryRecord.Count(h => h.Url.Contains("~unsounded")));
        Assert.AreEqual(229, loadedHistoryRecord.Count(h => h.Url.Contains("/~zed-and-syndra/")));
        Assert.AreEqual(198, loadedHistoryRecord.Count(h => h.Url.Contains("~zed-and-syndra-2")));
        Assert.AreEqual(166, loadedHistoryRecord.Count(h => h.Url.Contains("~finders-keepers")));
        Assert.AreEqual(60, loadedHistoryRecord.Count(h => h.Url.Contains("pine_in_the_flower_garden")));
        Assert.AreEqual(69, loadedHistoryRecord.Count(h => h.Url.Contains("legend_and_the_hero")));
        Assert.AreEqual(384, loadedHistoryRecord.Count(h => h.Url.Contains("gantz")));
        Assert.AreEqual(59, loadedHistoryRecord.Count(h => h.Url.Contains("addicted_to_curry")));
        Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("knight_run")));
        Assert.AreEqual(47, loadedHistoryRecord.Count(h => h.Url.Contains("dears")));
        Assert.AreEqual(119, loadedHistoryRecord.Count(h => h.Url.Contains("rough")));
        Assert.AreEqual(88, loadedHistoryRecord.Count(h => h.Url.Contains("the_shining_wind")));
        Assert.AreEqual(53, loadedHistoryRecord.Count(h => h.Url.Contains("gun_dream")));
        Assert.AreEqual(37, loadedHistoryRecord.Count(h => h.Url.Contains("hwaja")));
        Assert.AreEqual(52, loadedHistoryRecord.Count(h => h.Url.Contains("limited_weapon")));
        Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("good_morning_call")));
        Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("kare_kano__his_and_her_circumstances")));
        Assert.AreEqual(39, loadedHistoryRecord.Count(h => h.Url.Contains("monthly_girls__nozaki_kun")));
        Assert.AreEqual(54, loadedHistoryRecord.Count(h => h.Url.Contains("candy_candy")));
        Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("mushishi")));
        Assert.AreEqual(82, loadedHistoryRecord.Count(h => h.Url.Contains("minami_ke")));
        Assert.AreEqual(42, loadedHistoryRecord.Count(h => h.Url.Contains("ikigami")));
        Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("cross_game")));

        Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());
        Assert.AreEqual(3606, loadedHistoryRecord.Count);
      }
    }

    [SetUp]
    public void Clean()
    {
      Environment.SetUp(false);
      Environment.BeforeTestClean();
    }

    [TearDown]
    public void Close()
    {
      Environment.TestCleanup();
    }
  }
}
