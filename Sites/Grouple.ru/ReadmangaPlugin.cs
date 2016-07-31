using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class ReadmangaPlugin : BasePlugin
  {
    public override string ShortName { get { return "RM"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override Guid MangaGuid { get { return Readmanga.Type; } }
    public override Type MangaType { get { return typeof(Readmanga); } }
    public override Guid LoginGuid { get { return GroupleLogin.Type; } }
    public override Login GetLogin()
    {
      return Login.Get<GroupleLogin>();
    }
  }
}