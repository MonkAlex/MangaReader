using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentai2Read.com
{
  [Export(typeof(IPlugin))]
  public class Hentai2ReadPlugin : BasePlugin<Hentai2ReadPlugin>
  {
    public override string ShortName { get { return "H2R"; } }

    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return new Guid("410C7DCE-B452-41CA-A348-62A232F11D5A"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof(Hentai2Read); } }
    public override Type LoginType { get { return typeof(Hentai2ReadLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override CookieClient GetCookieClient()
    {
      var mainUri = Generic.GetLoginMainUri<Hentai2Read>();
      var client = new Hentai2ReadClient
      {
        BaseAddress = mainUri.OriginalString,
        Cookie = CookieContainer
      };
      return client;
    }
    public override ISiteParser GetParser()
    {
      return new Hentai2ReadParser();
    }
  }
}
