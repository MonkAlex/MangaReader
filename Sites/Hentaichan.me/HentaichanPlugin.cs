using System;
using System.ComponentModel.Composition;
using MangaReader.Core;

namespace Hentaichan
{
  [Export(typeof(IPlugin))]
  public class HentaichanPlugin : HentaichanBasePlugin<HentaichanPlugin>
  {
    public override string ShortName { get { return "HC"; } }
    public static Guid Manga { get { return Guid.Parse("6F2A3ACC-70B2-4FF3-9BCB-0E3D15971FDE"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type LoginType { get { return typeof(HentaichanLogin); } }
    public override Type MangaType { get { return typeof(Hentaichan); } }
    public override ISiteParser GetParser()
    {
      return new Parser();
    }
  }
}
