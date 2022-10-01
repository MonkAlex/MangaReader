using System;
using System.Reflection;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services.Config;

namespace Grouple
{
  public abstract class GrouplePlugin<T> : BasePlugin<T>, IGrouplePlugin where T : class, IPlugin
  {
    protected GrouplePlugin(PluginManager pluginManager, Config config) : base(pluginManager, config)
    {
    }

    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }

    /// <summary>
    /// Используется в авторизации
    /// </summary>

    public abstract ushort SiteId { get; }

    protected override Task ConfigureCookieClient(ISiteHttpClient client, ILogin login)
    {
      client.AddCookie(GroupleParser.CookieKey, "true");
      return base.ConfigureCookieClient(client, login);
    }
  }

  internal interface IGrouplePlugin : IPlugin
  {
    ushort SiteId { get; }
  }
}
