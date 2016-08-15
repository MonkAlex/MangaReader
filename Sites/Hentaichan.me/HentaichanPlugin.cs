using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;

namespace Hentaichan
{
  [Export(typeof(IPlugin))]
  public class HentaichanPlugin : BasePlugin
  {
    public override string ShortName { get {return "HC"; } }

    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }

    public override Guid MangaGuid { get { return Hentaichan.Type; } }

    public override Type LoginType { get { return typeof(HentaichanLogin); } }

    public override Type MangaType { get { return typeof (Hentaichan); } }
  }
}