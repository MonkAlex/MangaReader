using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan
{
  [Export(typeof(IPlugin))]
  public class HentaichanPlugin : BasePlugin<HentaichanPlugin>
  {
    public override string ShortName { get { return "HC"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return Guid.Parse("6F2A3ACC-70B2-4FF3-9BCB-0E3D15971FDE"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type LoginType { get { return typeof(HentaichanLogin); } }
    public override Type MangaType { get { return typeof(Hentaichan); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override CookieClient GetCookieClient()
    {
      var mainUri = Generic.GetLoginMainUri<Hentaichan>();
      var client = new HentaichanClient
      {
        BaseAddress = mainUri.OriginalString,
        Cookie = CookieContainer
      };
      var setting = ConfigStorage.GetPlugin<Hentaichan>().GetSettings();
      if (setting != null)
      {
        var login = (HentaichanLogin)setting.Login;
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
      return new Parser();
    }
  }
}
