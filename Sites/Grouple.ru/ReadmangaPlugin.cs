using System;
using System.ComponentModel.Composition;
using MangaReader.Core;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class ReadmangaPlugin : GrouplePlugin<ReadmangaPlugin>
  {
    public override string ShortName { get { return "RM"; } }
    public static Guid Manga { get { return Guid.Parse("2C98BBF4-DB46-47C4-AB0E-F207E283142D"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof(Readmanga); } }
    public override ISiteParser GetParser()
    {
      return new ReadmangaParser();
    }
  }
}
