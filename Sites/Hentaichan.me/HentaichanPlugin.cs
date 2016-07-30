using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace Hentaichan
{
  [Export(typeof(IPlugin))]
  public class HentaichanPlugin : IPlugin
  {
    public Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }

    public Guid MangaGuid { get { return Hentaichan.Type; } }

    public Guid LoginGuid { get { return HentaichanLogin.Type; } }

    public Type MangaType { get { return typeof (Hentaichan); } }

    public Login GetLogin()
    {
      return Login.Get<HentaichanLogin>();
    }

    public MangaSetting GetSettings()
    {
      return ConfigStorage.Instance.DatabaseConfig.MangaSettings.Single(m => Equals(m.Manga, this.MangaGuid));
    }
  }
}