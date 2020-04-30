using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class MintmangaPlugin : BasePlugin<MintmangaPlugin>
  {
    public override string ShortName { get { return "MM"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return Guid.Parse("64AC91EF-BDB3-4086-BE17-BB1DBE7A7656"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof(Mintmanga); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
    public override CookieClient GetCookieClient()
    {
      var mainUri = Generic.GetLoginMainUri<Mintmanga>();
      var client = new MintmangaClient
      {
        BaseAddress = mainUri.OriginalString,
        Cookie = CookieContainer
      };
      return client;
    }

    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override ISiteParser GetParser()
    {
      return new MintmangaParser();
    }
  }
}
