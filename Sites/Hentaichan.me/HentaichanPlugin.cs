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
    public static Guid Manga { get { return Guid.Parse("6F2A3ACC-70B2-4FF3-9BCB-0E3D15971FDE"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type LoginType { get { return typeof(HentaichanLogin); } }
    public override Type MangaType { get { return typeof (Hentaichan); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override ISiteParser GetParser()
    {
      return new Parser();
    }
  }
}