using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public abstract class HentaichanBasePlugin<T> : BasePlugin<T> where T : class, IPlugin, new()
  {
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    protected override Task ConfigureCookieClient(ISiteHttpClient client, ILogin login)
    {
      var baseLogin = (BaseLogin)login;
      if (!string.IsNullOrWhiteSpace(baseLogin.UserId) && !client.GetCookies().Any(c => c.Name == "dle_user_id"))
      {
        client.AddCookie("dle_user_id", baseLogin.UserId);
        client.AddCookie("dle_password", baseLogin.PasswordHash);
      }

      return base.ConfigureCookieClient(client, login);
    }
  }
}
