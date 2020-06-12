using System;
using System.Net;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Grouple
{
  public abstract class GrouplePlugin<T> : BasePlugin<T> where T : class, IPlugin, new()
  {
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    protected override void ConfigureCookieClient(CookieClient client, Uri mainUri, MangaSetting setting)
    {
      var cookie = new Cookie
      {
        Name = GroupleParser.CookieKey,
        Value = "true",
        Expires = DateTime.Today.AddYears(1),
        Domain = "." + mainUri.Host
      };
      client.Cookie.Add(cookie);
    }
  }
}
