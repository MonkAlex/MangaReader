using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Acomics
{
  [Export(typeof(IPlugin))]
  public class AcomicsPlugin : IPlugin
  {
    public Assembly Assembly { get {return Assembly.GetAssembly(this.GetType());} }
    public Guid MangaGuid { get { return Acomics.Type; } }
    public Type MangaType { get { return typeof (Acomics); } }
    public Guid LoginGuid { get { return AcomicsLogin.Type; } }
    public Login GetLogin()
    {
      return Login.Get<AcomicsLogin>();
    }

    public MangaSetting GetSettings()
    {
      return ConfigStorage.Instance.DatabaseConfig.MangaSettings.SingleOrDefault(s => s.Manga == this.MangaGuid);
    }
  }
}