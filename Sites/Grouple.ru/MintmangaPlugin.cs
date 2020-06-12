using System;
using System.ComponentModel.Composition;
using MangaReader.Core;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class MintmangaPlugin : GrouplePlugin<MintmangaPlugin>
  {
    public override string ShortName { get { return "MM"; } }
    public static Guid Manga { get { return Guid.Parse("64AC91EF-BDB3-4086-BE17-BB1DBE7A7656"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof(Mintmanga); } }
    public override ISiteParser GetParser()
    {
      return new MintmangaParser();
    }
  }
}
