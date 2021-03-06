﻿using System;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;

namespace Grouple
{
  public abstract class GrouplePlugin<T> : BasePlugin<T> where T : class, IPlugin, new()
  {
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    protected override void ConfigureCookieClient(ISiteHttpClient client, ILogin login)
    {
      client.AddCookie(GroupleParser.CookieKey, "true");
    }
  }
}
