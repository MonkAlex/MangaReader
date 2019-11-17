using System;
using System.Linq;
using System.Reflection;

namespace Tests.Entities
{
  public static class MangaInfos
  {
    public static class Acomics
    {
      [InfoCache("https://acomics.ru/~MGS-LDIOH", true)]
      public static MangaInfo MgsLdioh;

      [InfoCache("https://acomics.ru/~supersciencefriends", false)]
      public static MangaInfo SuperScienceFriends;
    }

    public static class Henchan
    {
      [InfoCache("https://h-chan.me/manga/12850-twisted-intent-chast-1.html", true)]
      public static MangaInfo TwistedIntent;

      [InfoCache("https://h-chan.me/manga/14212-love-and-devil-glava-25.html", false)]
      public static MangaInfo LoveAndDevil;
    }

    public static class Mangachan
    {
      [InfoCache("https://manga-chan.me/manga/35617--rain-.html", true)]
      public static MangaInfo Rain;

      [InfoCache("https://manga-chan.me/manga/15659-this-girlfriend-is-fiction.html", false)]
      public static MangaInfo ThisGirlfriendIsFiction;

      [InfoCache("https://manga-chan.me/manga/64187-eve-scramble.html", false)]
      public static MangaInfo EveScramble;
    }

    public static class Readmanga
    {
      [InfoCache("https://readmanga.me/kuroshitsuji_dj___black_sheep", true)]
      public static MangaInfo Kuroshitsuji;

      [InfoCache("https://readmanga.me/love_mate_2", false)]
      public static MangaInfo LoveMate2;
    }

    public static class Mintmanga
    {
      [InfoCache("https://mintmanga.live/harukana_receive", true)]
      public static MangaInfo HarukaNaReceive;

      [InfoCache("https://mintmanga.live/love_mate", false)]
      public static MangaInfo LoveMate;
    }

    public static class Hentai2Read
    {
      [InfoCache("https://hentai2read.com/attention_please_yamashita_shunya/", true)]
      public static MangaInfo AttentionPlease;
    }

    public static void Init()
    {

    }

    static MangaInfos()
    {
      var fields = typeof(MangaInfos).GetNestedTypes().SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public));
      foreach (var field in fields)
      {
        field.SetValue(null, Builder.LoadFromCache(field.GetCustomAttribute<InfoCacheAttribute>()));
      }
    }
  }

  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public sealed class InfoCacheAttribute : Attribute
  {
    public string Uri;

    public bool Downloadable;

    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    public InfoCacheAttribute(string uri, bool downloadable)
    {
      Uri = uri;
      Downloadable = downloadable;
    }
  }
}
