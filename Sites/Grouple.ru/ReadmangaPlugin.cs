using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class ReadmangaPlugin : BasePlugin<ReadmangaPlugin>
  {
    public override string ShortName { get { return "RM"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public static Guid Manga { get { return Guid.Parse("2C98BBF4-DB46-47C4-AB0E-F207E283142D"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof(Readmanga); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Chapter; } }
    public override CookieClient GetCookieClient()
    {
      var mainUri = Generic.GetLoginMainUri<Readmanga>();
      var client = new ReadmangaClient
      {
        BaseAddress = mainUri.OriginalString,
        Cookie = CookieContainer
      };
      return client;
    }
    public override ISiteParser GetParser()
    {
      return new ReadmangaParser();
    }
  }
}
