﻿using System;
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
    protected override void ConfigureCookieClient(CookieClient client, Uri mainUri, MangaSetting setting)
    {
      var login = (BaseLogin)setting.Login;
      if (!string.IsNullOrWhiteSpace(login.UserId) && !client.Cookie.GetCookies(mainUri).Cast<Cookie>().Any(c => c.Name == "dle_user_id"))
      {
        client.Cookie.Add(new Cookie("dle_user_id", login.UserId, "/", mainUri.Host));
        client.Cookie.Add(new Cookie("dle_password", login.PasswordHash, "/", mainUri.Host));
      }
    }
  }
}