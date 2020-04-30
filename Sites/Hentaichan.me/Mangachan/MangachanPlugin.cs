using System;
using System.ComponentModel.Composition;
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
      var host = Generic.GetLoginMainUri<Mangachan>().Host;
      var client = new MangachanClient();
      client.BaseAddress = host;
      var setting = ConfigStorage.GetPlugin<Mangachan>().GetSettings();
      if (setting != null)
      {
        var login = (MangachanLogin)setting.Login;
        if (!login.CanLogin || string.IsNullOrWhiteSpace(login.UserId))
        {
          login.DoLogin(Manga).Wait();
        }
        if (!string.IsNullOrWhiteSpace(login.UserId))
        {
          client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", host));
          client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", host));
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
