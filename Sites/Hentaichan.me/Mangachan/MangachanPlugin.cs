using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan.Mangachan
{
  [Export(typeof(IPlugin))]
  public class MangachanPlugin : BasePlugin<MangachanPlugin>
  {
    public override string ShortName { get { return "MC"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return Guid.Parse("495FFD4F-D37F-437F-870A-5C1C321D1B20"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type LoginType { get { return typeof(MangachanLogin); } }
    public override Type MangaType { get { return typeof(Mangachan); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override CookieClient GetCookieClient()
    {
      var mainUri = Generic.GetLoginMainUri<Mangachan>();
      var client = new MangachanClient
      {
        BaseAddress = mainUri.OriginalString,
        Cookie = CookieContainer
      };
      var setting = ConfigStorage.GetPlugin<Mangachan>().GetSettings();
      if (setting != null)
      {
        var login = (MangachanLogin)setting.Login;
        if (!string.IsNullOrWhiteSpace(login.UserId) && !client.Cookie.GetCookies(mainUri).Cast<Cookie>().Any(c => c.Name == "dle_user_id"))
        {
          client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", mainUri.Host));
          client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", mainUri.Host));
        }
      }
      return client;
    }
    public override ISiteParser GetParser()
    {
      return new global::Hentaichan.Mangachan.Parser();
    }
  }
}
