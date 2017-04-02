using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;

namespace Hentaichan.Mangachan
{
  [Export(typeof(IPlugin))]
  public class MangachanPlugin : BasePlugin
  {
    public override string ShortName { get { return "MC"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return Guid.Parse("495FFD4F-D37F-437F-870A-5C1C321D1B20"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type LoginType { get { return typeof(MangachanLogin); } }
    public override Type MangaType { get { return typeof(Mangachan); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override ISiteParser GetParser()
    {
      return new global::Hentaichan.Mangachan.Parser();
    }
  }
}
