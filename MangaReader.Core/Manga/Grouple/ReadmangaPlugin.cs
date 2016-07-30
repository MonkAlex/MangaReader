using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga.Grouple
{
  [Export(typeof(IPlugin))]
  public class ReadmangaPlugin : IPlugin
  {
    public Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public Guid MangaGuid { get { return Readmanga.Type; } }
    public Type MangaType { get { return typeof(Readmanga); } }
    public Guid LoginGuid { get { return GroupleLogin.Type; } }
    public Login GetLogin()
    {
      return Login.Get<GroupleLogin>();
    }

    public MangaSetting GetSettings()
    {
      return ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => s.Manga == this.MangaGuid);
    }
  }
}