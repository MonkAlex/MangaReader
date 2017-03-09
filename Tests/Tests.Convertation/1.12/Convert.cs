using System.Linq;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Tests.Convertation._1._12
{
  [TestClass]
  public class Convert
  {
    [TestMethod, Ignore]
    public void Convert_1_12_to_main()
    {
      Environment.Deploy(@"Tests.Convertation\1.12");

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      var loadedMangas = MangaReader.Core.NHibernate.Mapping.Session.Query<IManga>().ToList();
      Assert.AreEqual(75, loadedMangas.Count);

      var loadedHistoryRecord = MangaReader.Core.NHibernate.Mapping.Session.Query<MangaHistory>().ToList();

      /*
      var s = loadedMangas.Select(m => $"Assert.AreEqual({m.Histories.Count()}, loadedHistoryRecord.Count(h => h.Url.Contains(\"{m.Uri.OriginalString.Split('/').Last()}\")));").ToList();
      s.Add($"Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());");
      System.IO.File.WriteAllLines(@"D:\test.code", s);
      */

      Assert.AreEqual(42, loadedHistoryRecord.Count(h => h.Url.Contains("3x3_eyes")));
      Assert.AreEqual(148, loadedHistoryRecord.Count(h => h.Url.Contains("oh_my_goddess")));
      Assert.AreEqual(33, loadedHistoryRecord.Count(h => h.Url.Contains("anagle_mole")));
      Assert.AreEqual(36, loadedHistoryRecord.Count(h => h.Url.Contains("area_d")));
      Assert.AreEqual(22, loadedHistoryRecord.Count(h => h.Url.Contains("see_me_after_class")));
      Assert.AreEqual(113, loadedHistoryRecord.Count(h => h.Url.Contains("evil_blade")));
      Assert.AreEqual(226, loadedHistoryRecord.Count(h => h.Url.Contains("b_type_h_style")));
      Assert.AreEqual(7, loadedHistoryRecord.Count(h => h.Url.Contains("baka_and_boing")));
      Assert.AreEqual(20, loadedHistoryRecord.Count(h => h.Url.Contains("the_idiot__the_test__and_summoned_creatures")));
      Assert.AreEqual(18, loadedHistoryRecord.Count(h => h.Url.Contains("berserk")));
      Assert.AreEqual(52, loadedHistoryRecord.Count(h => h.Url.Contains("the_god_of_poverty_is")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("i_love_my_little_sister")));
      Assert.AreEqual(6, loadedHistoryRecord.Count(h => h.Url.Contains("come_come_vanilla")));
      Assert.AreEqual(55, loadedHistoryRecord.Count(h => h.Url.Contains("deadman_wonderland")));
      Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("he_is_a_ultimate_teacher")));
      Assert.AreEqual(90, loadedHistoryRecord.Count(h => h.Url.Contains("cage_of_eden")));
      Assert.AreEqual(361, loadedHistoryRecord.Count(h => h.Url.Contains("fairytail")));
      Assert.AreEqual(92, loadedHistoryRecord.Count(h => h.Url.Contains("freezing")));
      Assert.AreEqual(54, loadedHistoryRecord.Count(h => h.Url.Contains("frogman")));
      Assert.AreEqual(32, loadedHistoryRecord.Count(h => h.Url.Contains("pals_with_fujimura_kun")));
      Assert.AreEqual(10, loadedHistoryRecord.Count(h => h.Url.Contains("goudere_bishoujo_nagihara_sora")));
      Assert.AreEqual(14, loadedHistoryRecord.Count(h => h.Url.Contains("happy_negative_marriage")));
      Assert.AreEqual(10, loadedHistoryRecord.Count(h => h.Url.Contains("between_haru_and_natsu__i_am")));
      Assert.AreEqual(409, loadedHistoryRecord.Count(h => h.Url.Contains("hayate_the_combat_butler")));
      Assert.AreEqual(26, loadedHistoryRecord.Count(h => h.Url.Contains("high_school_dxd")));
      Assert.AreEqual(32, loadedHistoryRecord.Count(h => h.Url.Contains("lucifer_and_the_biscuit_hammer")));
      Assert.AreEqual(113, loadedHistoryRecord.Count(h => h.Url.Contains("i_am_a_hero")));
      Assert.AreEqual(153, loadedHistoryRecord.Count(h => h.Url.Contains("strawberry_100")));
      Assert.AreEqual(5, loadedHistoryRecord.Count(h => h.Url.Contains("amnesia_labyrinth")));
      Assert.AreEqual(252, loadedHistoryRecord.Count(h => h.Url.Contains("the_world_god_only_knows")));
      Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("kiba_no_tabishounin___the_arms_peddler")));
      Assert.AreEqual(211, loadedHistoryRecord.Count(h => h.Url.Contains("a_town_where_you_live")));
      Assert.AreEqual(70, loadedHistoryRecord.Count(h => h.Url.Contains("kissxsis")));
      Assert.AreEqual(17, loadedHistoryRecord.Count(h => h.Url.Contains("coloured_in_love_maple")));
      Assert.AreEqual(18, loadedHistoryRecord.Count(h => h.Url.Contains("this_older_woman_is_a_fiction")));
      Assert.AreEqual(119, loadedHistoryRecord.Count(h => h.Url.Contains("take_a_look_at_this_s")));
      Assert.AreEqual(55, loadedHistoryRecord.Count(h => h.Url.Contains("kure_nai")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("kurogane__kezawa_haruto")));
      Assert.AreEqual(107, loadedHistoryRecord.Count(h => h.Url.Contains("black_god")));
      Assert.AreEqual(128, loadedHistoryRecord.Count(h => h.Url.Contains("mangaka_san_to_assistant_san_to")));
      Assert.AreEqual(76, loadedHistoryRecord.Count(h => h.Url.Contains("minamoto_kun_s_story")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("daily_life_with_a_monster_girl")));
      Assert.AreEqual(162, loadedHistoryRecord.Count(h => h.Url.Contains("nagasarete_airantou")));
      Assert.AreEqual(80, loadedHistoryRecord.Count(h => h.Url.Contains("nana_and_kaoru")));
      Assert.AreEqual(65, loadedHistoryRecord.Count(h => h.Url.Contains("my_mysterious_girlfriend_x")));
      Assert.AreEqual(79, loadedHistoryRecord.Count(h => h.Url.Contains("false_love")));
      Assert.AreEqual(107, loadedHistoryRecord.Count(h => h.Url.Contains("a_peephole")));
      Assert.AreEqual(7, loadedHistoryRecord.Count(h => h.Url.Contains("nyatto")));
      Assert.AreEqual(37, loadedHistoryRecord.Count(h => h.Url.Contains("nyotai_ka")));
      Assert.AreEqual(708, loadedHistoryRecord.Count(h => h.Url.Contains("one__piece")));
      Assert.AreEqual(8, loadedHistoryRecord.Count(h => h.Url.Contains("listen_to_what_papa_says")));
      Assert.AreEqual(51, loadedHistoryRecord.Count(h => h.Url.Contains("pastel")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("rika")));
      Assert.AreEqual(66, loadedHistoryRecord.Count(h => h.Url.Contains("rosario_to_vampire_ii")));
      Assert.AreEqual(60, loadedHistoryRecord.Count(h => h.Url.Contains("sakuranbo_syndrome")));
      Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("sankarea")));
      Assert.AreEqual(68, loadedHistoryRecord.Count(h => h.Url.Contains("lost_property_of_the_sky")));
      Assert.AreEqual(37, loadedHistoryRecord.Count(h => h.Url.Contains("say__i_love_you")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("sundome")));
      Assert.AreEqual(105, loadedHistoryRecord.Count(h => h.Url.Contains("love_mate")));
      Assert.AreEqual(46, loadedHistoryRecord.Count(h => h.Url.Contains("dusk_maiden_of_amnesia")));
      Assert.AreEqual(21, loadedHistoryRecord.Count(h => h.Url.Contains("birdy_the_mighty_2")));
      Assert.AreEqual(118, loadedHistoryRecord.Count(h => h.Url.Contains("the_breaker__new_waves")));
      Assert.AreEqual(101, loadedHistoryRecord.Count(h => h.Url.Contains("the_one")));
      Assert.AreEqual(49, loadedHistoryRecord.Count(h => h.Url.Contains("to_love_ru_darkness")));
      Assert.AreEqual(35, loadedHistoryRecord.Count(h => h.Url.Contains("the_monster_next_to_me")));
      Assert.AreEqual(209, loadedHistoryRecord.Count(h => h.Url.Contains("hooligan_and_wearing_spectacles")));
      Assert.AreEqual(14, loadedHistoryRecord.Count(h => h.Url.Contains("yomeiro_choice")));
      Assert.AreEqual(57, loadedHistoryRecord.Count(h => h.Url.Contains("dream_eater_merry")));
      Assert.AreEqual(24, loadedHistoryRecord.Count(h => h.Url.Contains("yuria_type_100")));
      Assert.AreEqual(78, loadedHistoryRecord.Count(h => h.Url.Contains("zippy_ziggy")));
      Assert.AreEqual(52, loadedHistoryRecord.Count(h => h.Url.Contains("btooom_")));
      Assert.AreEqual(34, loadedHistoryRecord.Count(h => h.Url.Contains("blood_lad")));
      Assert.AreEqual(88, loadedHistoryRecord.Count(h => h.Url.Contains("ocean_s_cape")));
      Assert.AreEqual(2, loadedHistoryRecord.Count(h => h.Url.Contains("fisheye_placebo")));

      Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count);
      Assert.AreEqual(6129, loadedHistoryRecord.Count);
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