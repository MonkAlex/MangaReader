using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace Tests.Convertation._1._12
{
  [TestFixture]
  public class Convert
  {
    [Test]
    public void Convert_1_12_to_main()
    {
      Environment.DeployToRoot(Path.Combine("Tests.Convertation", "1.12"));

      var startDate = DateTime.Now;

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      using (var context = Repository.GetEntityContext())
      {
        var loadedMangas = context.Get<IManga>().ToList();
        Assert.AreEqual(32, loadedMangas.Count);

        var loadedHistoryRecord = context.Get<MangaHistory>().ToList();

        /*
        var s = loadedMangas.Select(m => $"Assert.AreEqual({m.Histories.Count()}, loadedHistoryRecord.Count(h => h.Url.Contains(\"{m.Uri.OriginalString.Split('/').Last()}\")));").ToList();
        s.Add($"Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());");
        System.IO.File.WriteAllLines(@"D:\test.code", s);
        */

        Assert.AreEqual(42, loadedHistoryRecord.Count(h => h.Url.Contains("3x3_eyes")));
        Assert.AreEqual(148, loadedHistoryRecord.Count(h => h.Url.Contains("oh__my_goddess_")));
        Assert.AreEqual(33, loadedHistoryRecord.Count(h => h.Url.Contains("anagle_mole")));
        Assert.AreEqual(36, loadedHistoryRecord.Count(h => h.Url.Contains("area_d")));
        Assert.AreEqual(226, loadedHistoryRecord.Count(h => h.Url.Contains("b_type_h_style")));
        Assert.AreEqual(7, loadedHistoryRecord.Count(h => h.Url.Contains("baka_and_boing")));
        Assert.AreEqual(20, loadedHistoryRecord.Count(h => h.Url.Contains("baka_and_test__summon_the_beasts")));
        Assert.AreEqual(52, loadedHistoryRecord.Count(h => h.Url.Contains("the_god_of_poverty_is")));
        Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("i_love_my_little_sister")));
        Assert.AreEqual(6, loadedHistoryRecord.Count(h => h.Url.Contains("come_and_wild_vanilla")));
        Assert.AreEqual(55, loadedHistoryRecord.Count(h => h.Url.Contains("deadman_wonderland")));
        Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("he_is_a_ultimate_teacher")));
        Assert.AreEqual(90, loadedHistoryRecord.Count(h => h.Url.Contains("cage_of_eden")));
        Assert.AreEqual(361, loadedHistoryRecord.Count(h => h.Url.Contains("fairytail")));
        Assert.AreEqual(92, loadedHistoryRecord.Count(h => h.Url.Contains("freezing")));
        Assert.AreEqual(54, loadedHistoryRecord.Count(h => h.Url.Contains("frogman")));
        Assert.AreEqual(32, loadedHistoryRecord.Count(h => h.Url.Contains("pals_with_fujimura_kun")));
        Assert.AreEqual(10, loadedHistoryRecord.Count(h => h.Url.Contains("gou_dere_sora_nagihara")));
        Assert.AreEqual(14, loadedHistoryRecord.Count(h => h.Url.Contains("happy_negative_marriage")));
        Assert.AreEqual(10, loadedHistoryRecord.Count(h => h.Url.Contains("between_haru_and_natsu__i_am")));
        Assert.AreEqual(409, loadedHistoryRecord.Count(h => h.Url.Contains("hayate_the_combat_butler")));
        Assert.AreEqual(26, loadedHistoryRecord.Count(h => h.Url.Contains("high_school_dxd")));
        Assert.AreEqual(32, loadedHistoryRecord.Count(h => h.Url.Contains("lucifer_and_the_biscuit_hammer")));
        Assert.AreEqual(113, loadedHistoryRecord.Count(h => h.Url.Contains("i_am_a_hero")));
        Assert.AreEqual(153, loadedHistoryRecord.Count(h => h.Url.Contains("strawberry_100")));
        Assert.AreEqual(5, loadedHistoryRecord.Count(h => h.Url.Contains("amnesia_labyrinth")));
        Assert.AreEqual(252, loadedHistoryRecord.Count(h => h.Url.Contains("the_world_god_only_knows")));
        Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("the_arms_peddler")));
        Assert.AreEqual(211, loadedHistoryRecord.Count(h => h.Url.Contains("a_town_where_you_live")));
        Assert.AreEqual(70, loadedHistoryRecord.Count(h => h.Url.Contains("kissxsis")));
        Assert.AreEqual(17, loadedHistoryRecord.Count(h => h.Url.Contains("coloured_in_love_maple")));
        Assert.AreEqual(18, loadedHistoryRecord.Count(h => h.Url.Contains("this_older_woman_is_a_fiction")));

        Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count);
        Assert.AreEqual(2754, loadedHistoryRecord.Count);
        // 1.12 history create records now.
        Assert.Less(startDate, loadedMangas.Min(m => m.Created));
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