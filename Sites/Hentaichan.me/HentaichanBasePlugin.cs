using System;
using System.Linq;
using System.Net;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public abstract class HentaichanBasePlugin<T> : BasePlugin<T> where T : class, IPlugin, new()
  {
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    protected override void ConfigureCookieClient(ISiteHttpClient client, Uri mainUri, ILogin login)
    {
      var baseLogin = (BaseLogin)login;
      if (!string.IsNullOrWhiteSpace(baseLogin.UserId) && !client.CookieContainer.GetCookies(mainUri).Cast<Cookie>().Any(c => c.Name == "dle_user_id"))
      {
        client.CookieContainer.Add(new Cookie("dle_user_id", baseLogin.UserId, "/", mainUri.Host));
        client.CookieContainer.Add(new Cookie("dle_password", baseLogin.PasswordHash, "/", mainUri.Host));
      }
    }
  }
}
