using System.Linq;
using MangaReader.Core.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;

namespace Tests.Convertation._1._20
{
  [TestClass]
  public class Convert
  {
    [TestMethod, Ignore]
    public void Convert_1_20_to_main()
    {
      Environment.Deploy(@"Tests.Convertation\1.20");

      MangaReader.Core.Client.Init();
      MangaReader.Core.Client.Start(new ReportProcess());

      var loadedMangas = MangaReader.Core.NHibernate.Mapping.Session.Query<IManga>().ToList();
      Assert.AreEqual(159, loadedMangas.Count);

      var loadedHistoryRecord = MangaReader.Core.NHibernate.Mapping.Session.Query<MangaHistory>().ToList();

      /*
      var s = loadedMangas.Select(m => $"Assert.AreEqual({m.Histories.Count()}, loadedHistoryRecord.Count(h => h.Url.Contains(\"{m.Uri.OriginalString.Split('/').Last()}\")));").ToList();
      s.Add($"Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());");
      System.IO.File.WriteAllLines(@"D:\test.code", s);
      */

      Assert.AreEqual(188, loadedHistoryRecord.Count(h => h.Url.Contains("3x3_eyes")));
      Assert.AreEqual(177, loadedHistoryRecord.Count(h => h.Url.Contains("oh_my_goddess")));
      Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("anagle_mole")));
      Assert.AreEqual(151, loadedHistoryRecord.Count(h => h.Url.Contains("area_d")));
      Assert.AreEqual(24, loadedHistoryRecord.Count(h => h.Url.Contains("asa_made_jugyou_chu")));
      Assert.AreEqual(7, loadedHistoryRecord.Count(h => h.Url.Contains("baka_and_boing")));
      Assert.AreEqual(26, loadedHistoryRecord.Count(h => h.Url.Contains("the_idiot__the_test__and_summoned_creatures")));
      Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("berserk")));
      Assert.AreEqual(82, loadedHistoryRecord.Count(h => h.Url.Contains("the_god_of_poverty_is")));
      Assert.AreEqual(19, loadedHistoryRecord.Count(h => h.Url.Contains("come_come_vanilla")));
      Assert.AreEqual(216, loadedHistoryRecord.Count(h => h.Url.Contains("he_is_a_ultimate_teacher")));
      Assert.AreEqual(150, loadedHistoryRecord.Count(h => h.Url.Contains("cage_of_eden")));
      Assert.AreEqual(476, loadedHistoryRecord.Count(h => h.Url.Contains("fairy_tail")));
      Assert.AreEqual(161, loadedHistoryRecord.Count(h => h.Url.Contains("freezing")));
      Assert.AreEqual(15, loadedHistoryRecord.Count(h => h.Url.Contains("goudere_bishoujo_nagihara_sora")));
      Assert.AreEqual(23, loadedHistoryRecord.Count(h => h.Url.Contains("happy_negative_marriage")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("between_haru_and_natsu__i_am")));
      Assert.AreEqual(483, loadedHistoryRecord.Count(h => h.Url.Contains("hayate_the_combat_butler")));
      Assert.AreEqual(33, loadedHistoryRecord.Count(h => h.Url.Contains("high_school_dxd")));
      Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("lucifer_and_the_biscuit_hammer")));
      Assert.AreEqual(163, loadedHistoryRecord.Count(h => h.Url.Contains("i_am_a_hero")));
      Assert.AreEqual(168, loadedHistoryRecord.Count(h => h.Url.Contains("strawberry_100")));
      Assert.AreEqual(5, loadedHistoryRecord.Count(h => h.Url.Contains("amnesia_labyrinth")));
      Assert.AreEqual(333, loadedHistoryRecord.Count(h => h.Url.Contains("the_world_god_only_knows")));
      Assert.AreEqual(56, loadedHistoryRecord.Count(h => h.Url.Contains("kiba_no_tabishounin___the_arms_peddler")));
      Assert.AreEqual(86, loadedHistoryRecord.Count(h => h.Url.Contains("kissxsis")));
      Assert.AreEqual(25, loadedHistoryRecord.Count(h => h.Url.Contains("this_older_woman_is_a_fiction")));
      Assert.AreEqual(155, loadedHistoryRecord.Count(h => h.Url.Contains("take_a_look_at_this_s")));
      Assert.AreEqual(73, loadedHistoryRecord.Count(h => h.Url.Contains("kurogane__kezawa_haruto")));
      Assert.AreEqual(128, loadedHistoryRecord.Count(h => h.Url.Contains("black_god")));
      Assert.AreEqual(10, loadedHistoryRecord.Count(h => h.Url.Contains("the_comic_artist_and_his_assistants_2")));
      Assert.AreEqual(243, loadedHistoryRecord.Count(h => h.Url.Contains("minamoto_kun_s_story")));
      Assert.AreEqual(40, loadedHistoryRecord.Count(h => h.Url.Contains("daily_life_with_a_monster_girl")));
      Assert.AreEqual(174, loadedHistoryRecord.Count(h => h.Url.Contains("nagasarete_airantou")));
      Assert.AreEqual(119, loadedHistoryRecord.Count(h => h.Url.Contains("nana_and_kaoru")));
      Assert.AreEqual(83, loadedHistoryRecord.Count(h => h.Url.Contains("my_mysterious_girlfriend_x")));
      Assert.AreEqual(171, loadedHistoryRecord.Count(h => h.Url.Contains("nisekoi__komi_naoshi")));
      Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("nyotai_ka")));
      Assert.AreEqual(855, loadedHistoryRecord.Count(h => h.Url.Contains("one__piece")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("listen_to_what_papa_says")));
      Assert.AreEqual(69, loadedHistoryRecord.Count(h => h.Url.Contains("pastel")));
      Assert.AreEqual(16, loadedHistoryRecord.Count(h => h.Url.Contains("rika")));
      Assert.AreEqual(75, loadedHistoryRecord.Count(h => h.Url.Contains("rosario_to_vampire_ii")));
      Assert.AreEqual(84, loadedHistoryRecord.Count(h => h.Url.Contains("sakuranbo_syndrome")));
      Assert.AreEqual(76, loadedHistoryRecord.Count(h => h.Url.Contains("sankarea")));
      Assert.AreEqual(95, loadedHistoryRecord.Count(h => h.Url.Contains("lost_property_of_the_sky")));
      Assert.AreEqual(47, loadedHistoryRecord.Count(h => h.Url.Contains("say__i_love_you")));
      Assert.AreEqual(69, loadedHistoryRecord.Count(h => h.Url.Contains("dusk_maiden_of_amnesia")));
      Assert.AreEqual(103, loadedHistoryRecord.Count(h => h.Url.Contains("birdy_the_mighty_2")));
      Assert.AreEqual(190, loadedHistoryRecord.Count(h => h.Url.Contains("the_breaker__new_waves")));
      Assert.AreEqual(79, loadedHistoryRecord.Count(h => h.Url.Contains("to_love_ru_darkness")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("the_monster_next_to_me")));
      Assert.AreEqual(33, loadedHistoryRecord.Count(h => h.Url.Contains("yomeiro_choice")));
      Assert.AreEqual(73, loadedHistoryRecord.Count(h => h.Url.Contains("dream_eater_merry")));
      Assert.AreEqual(27, loadedHistoryRecord.Count(h => h.Url.Contains("yuria_type_100")));
      Assert.AreEqual(68, loadedHistoryRecord.Count(h => h.Url.Contains("btoom")));
      Assert.AreEqual(46, loadedHistoryRecord.Count(h => h.Url.Contains("blood_lad")));
      Assert.AreEqual(122, loadedHistoryRecord.Count(h => h.Url.Contains("ocean_s_cape")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("fisheye_placebo")));
      Assert.AreEqual(351, loadedHistoryRecord.Count(h => h.Url.Contains("hunter_x_hunter")));
      Assert.AreEqual(218, loadedHistoryRecord.Count(h => h.Url.Contains("d_gray_man")));
      Assert.AreEqual(310, loadedHistoryRecord.Count(h => h.Url.Contains("toriko")));
      Assert.AreEqual(132, loadedHistoryRecord.Count(h => h.Url.Contains("nice_to_meet_you__kami_sama")));
      Assert.AreEqual(299, loadedHistoryRecord.Count(h => h.Url.Contains("rave_master")));
      Assert.AreEqual(181, loadedHistoryRecord.Count(h => h.Url.Contains("until_death_do_us_part")));
      Assert.AreEqual(154, loadedHistoryRecord.Count(h => h.Url.Contains("the_greatest_fusion_fantasy")));
      Assert.AreEqual(216, loadedHistoryRecord.Count(h => h.Url.Contains("liar_game")));
      Assert.AreEqual(51, loadedHistoryRecord.Count(h => h.Url.Contains("l_dk")));
      Assert.AreEqual(89, loadedHistoryRecord.Count(h => h.Url.Contains("/nana/")));
      Assert.AreEqual(143, loadedHistoryRecord.Count(h => h.Url.Contains("dorohedoro")));
      Assert.AreEqual(97, loadedHistoryRecord.Count(h => h.Url.Contains("kimi_ni_todoke")));
      Assert.AreEqual(102, loadedHistoryRecord.Count(h => h.Url.Contains("perfect_girl_evolution")));
      Assert.AreEqual(71, loadedHistoryRecord.Count(h => h.Url.Contains("break_blade")));
      Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("the_devil_and_her_love_song")));
      Assert.AreEqual(46, loadedHistoryRecord.Count(h => h.Url.Contains("dreamin__sun")));
      Assert.AreEqual(79, loadedHistoryRecord.Count(h => h.Url.Contains("you_are_my_pet")));
      Assert.AreEqual(68, loadedHistoryRecord.Count(h => h.Url.Contains("a_tackle_on_my_life")));
      Assert.AreEqual(69, loadedHistoryRecord.Count(h => h.Url.Contains("queen_s_knight")));
      Assert.AreEqual(56, loadedHistoryRecord.Count(h => h.Url.Contains("anima")));
      Assert.AreEqual(51, loadedHistoryRecord.Count(h => h.Url.Contains("a_night_of_a_thousand_dreams")));
      Assert.AreEqual(86, loadedHistoryRecord.Count(h => h.Url.Contains("gamble_fish")));
      Assert.AreEqual(155, loadedHistoryRecord.Count(h => h.Url.Contains("/monster/")));
      Assert.AreEqual(133, loadedHistoryRecord.Count(h => h.Url.Contains("akb49___the_rules_against_love")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("toradora")));
      Assert.AreEqual(121, loadedHistoryRecord.Count(h => h.Url.Contains("saki")));
      Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("love_attack__seino_shizuru")));
      Assert.AreEqual(89, loadedHistoryRecord.Count(h => h.Url.Contains("a_badboy_drinks_tea")));
      Assert.AreEqual(54, loadedHistoryRecord.Count(h => h.Url.Contains("enchained_spiritual_beast_ga_rei")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("school_shock")));
      Assert.AreEqual(28, loadedHistoryRecord.Count(h => h.Url.Contains("momoiro_heaven")));
      Assert.AreEqual(123, loadedHistoryRecord.Count(h => h.Url.Contains("homunculus")));
      Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("d_n_angel")));
      Assert.AreEqual(89, loadedHistoryRecord.Count(h => h.Url.Contains("dragon_head")));
      Assert.AreEqual(40, loadedHistoryRecord.Count(h => h.Url.Contains("june_the_little_queen")));
      Assert.AreEqual(99, loadedHistoryRecord.Count(h => h.Url.Contains("young_gto__shonan_junai_gumi")));
      Assert.AreEqual(142, loadedHistoryRecord.Count(h => h.Url.Contains("yokohama_shopping_trip")));
      Assert.AreEqual(85, loadedHistoryRecord.Count(h => h.Url.Contains("shibatora")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("ghost_byeolgok")));
      Assert.AreEqual(129, loadedHistoryRecord.Count(h => h.Url.Contains("working")));
      Assert.AreEqual(23, loadedHistoryRecord.Count(h => h.Url.Contains("crazy_for_you")));
      Assert.AreEqual(60, loadedHistoryRecord.Count(h => h.Url.Contains("in_the_first_grade")));
      Assert.AreEqual(42, loadedHistoryRecord.Count(h => h.Url.Contains("the_flower_crown_madonna")));
      Assert.AreEqual(51, loadedHistoryRecord.Count(h => h.Url.Contains("ng_life")));
      Assert.AreEqual(68, loadedHistoryRecord.Count(h => h.Url.Contains("melo_holic")));
      Assert.AreEqual(64, loadedHistoryRecord.Count(h => h.Url.Contains("parasite")));
      Assert.AreEqual(86, loadedHistoryRecord.Count(h => h.Url.Contains("angel_densetsu")));
      Assert.AreEqual(98, loadedHistoryRecord.Count(h => h.Url.Contains("today__we_ll_start_our_love")));
      Assert.AreEqual(39, loadedHistoryRecord.Count(h => h.Url.Contains("unbreakable_machine_doll")));
      Assert.AreEqual(173, loadedHistoryRecord.Count(h => h.Url.Contains("girl_the_wild_s")));
      Assert.AreEqual(96, loadedHistoryRecord.Count(h => h.Url.Contains("bloody_monday")));
      Assert.AreEqual(78, loadedHistoryRecord.Count(h => h.Url.Contains("to_aru_kagaku_no_railgun")));
      Assert.AreEqual(51, loadedHistoryRecord.Count(h => h.Url.Contains("/a_certain_magical_index/")));
      Assert.AreEqual(9, loadedHistoryRecord.Count(h => h.Url.Contains("a_certain_magical_index___the_miracle_of_endymion")));
      Assert.AreEqual(11, loadedHistoryRecord.Count(h => h.Url.Contains("to_aru_kagaku_no_accelerator")));
      Assert.AreEqual(557, loadedHistoryRecord.Count(h => h.Url.Contains("~flakypastry")));
      Assert.AreEqual(344, loadedHistoryRecord.Count(h => h.Url.Contains("~inferno")));
      Assert.AreEqual(676, loadedHistoryRecord.Count(h => h.Url.Contains("~unsounded")));
      Assert.AreEqual(229, loadedHistoryRecord.Count(h => h.Url.Contains("/~zed-and-syndra/")));
      Assert.AreEqual(198, loadedHistoryRecord.Count(h => h.Url.Contains("~zed-and-syndra-2")));
      Assert.AreEqual(166, loadedHistoryRecord.Count(h => h.Url.Contains("~finders-keepers")));
      Assert.AreEqual(60, loadedHistoryRecord.Count(h => h.Url.Contains("sol_in_the_flowerbed")));
      Assert.AreEqual(69, loadedHistoryRecord.Count(h => h.Url.Contains("legend_and_the_hero")));
      Assert.AreEqual(384, loadedHistoryRecord.Count(h => h.Url.Contains("gantz")));
      Assert.AreEqual(59, loadedHistoryRecord.Count(h => h.Url.Contains("addicted_to_curry")));
      Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("knight_run")));
      Assert.AreEqual(47, loadedHistoryRecord.Count(h => h.Url.Contains("dears")));
      Assert.AreEqual(119, loadedHistoryRecord.Count(h => h.Url.Contains("rough")));
      Assert.AreEqual(88, loadedHistoryRecord.Count(h => h.Url.Contains("the_shining_wind")));
      Assert.AreEqual(53, loadedHistoryRecord.Count(h => h.Url.Contains("battle_angel_alita")));
      Assert.AreEqual(37, loadedHistoryRecord.Count(h => h.Url.Contains("hwaja")));
      Assert.AreEqual(52, loadedHistoryRecord.Count(h => h.Url.Contains("limited_weapon")));
      Assert.AreEqual(41, loadedHistoryRecord.Count(h => h.Url.Contains("good_morning_call")));
      Assert.AreEqual(43, loadedHistoryRecord.Count(h => h.Url.Contains("kare_kano__his_and_her_circumstances")));
      Assert.AreEqual(39, loadedHistoryRecord.Count(h => h.Url.Contains("gekkan_shoujo_nozaki_kun")));
      Assert.AreEqual(54, loadedHistoryRecord.Count(h => h.Url.Contains("candy_candy")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("mushishi")));
      Assert.AreEqual(82, loadedHistoryRecord.Count(h => h.Url.Contains("minami_ke")));
      Assert.AreEqual(42, loadedHistoryRecord.Count(h => h.Url.Contains("ikigami")));
      Assert.AreEqual(50, loadedHistoryRecord.Count(h => h.Url.Contains("cross_game")));
      Assert.AreEqual(556, loadedHistoryRecord.Count(h => h.Url.Contains("strongest_disciple_kenichi")));
      Assert.AreEqual(150, loadedHistoryRecord.Count(h => h.Url.Contains("sun_ken_rock")));
      Assert.AreEqual(117, loadedHistoryRecord.Count(h => h.Url.Contains("wolf_guy___wolfen_crest")));
      Assert.AreEqual(245, loadedHistoryRecord.Count(h => h.Url.Contains("fist_of_the_north_star")));
      Assert.AreEqual(111, loadedHistoryRecord.Count(h => h.Url.Contains("the_seven_deadly_sins")));
      Assert.AreEqual(34, loadedHistoryRecord.Count(h => h.Url.Contains("dragon_who")));
      Assert.AreEqual(13, loadedHistoryRecord.Count(h => h.Url.Contains("multiplying_aizawa_san")));
      Assert.AreEqual(9, loadedHistoryRecord.Count(h => h.Url.Contains("twelve_beast")));
      Assert.AreEqual(24, loadedHistoryRecord.Count(h => h.Url.Contains("the_hentai_prince_and_the_stony_cat")));
      Assert.AreEqual(151, loadedHistoryRecord.Count(h => h.Url.Contains("saru_lock")));
      Assert.AreEqual(63, loadedHistoryRecord.Count(h => h.Url.Contains("otaku_s_daughter")));
      Assert.AreEqual(64, loadedHistoryRecord.Count(h => h.Url.Contains("the_new_secret_royal_commissioner")));
      Assert.AreEqual(49, loadedHistoryRecord.Count(h => h.Url.Contains("aflame_inferno")));
      Assert.AreEqual(113, loadedHistoryRecord.Count(h => h.Url.Contains("absolutely_lovely_children")));
      Assert.AreEqual(110, loadedHistoryRecord.Count(h => h.Url.Contains("team_medical_dragon")));
      Assert.AreEqual(130, loadedHistoryRecord.Count(h => h.Url.Contains("umisho")));
      Assert.AreEqual(40, loadedHistoryRecord.Count(h => h.Url.Contains("ichi_the_killer")));
      Assert.AreEqual(44, loadedHistoryRecord.Count(h => h.Url.Contains("virgin_with_a_wild_imagination")));
      Assert.AreEqual(67, loadedHistoryRecord.Count(h => h.Url.Contains("monochrome_factor")));
      Assert.AreEqual(68, loadedHistoryRecord.Count(h => h.Url.Contains("the_knight_of_cydonia")));

      Assert.AreEqual(loadedMangas.Sum(m => m.Histories.Count()), loadedHistoryRecord.Count());
      Assert.AreEqual(17839, loadedHistoryRecord.Count);
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
