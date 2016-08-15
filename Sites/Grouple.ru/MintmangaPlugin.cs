using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;

namespace Grouple
{
  [Export(typeof(IPlugin))]
  public class MintmangaPlugin : BasePlugin
  {
    public override string ShortName { get { return "MM"; } }
    public override Assembly Assembly { get { return Assembly.GetAssembly(this.GetType()); } }
    public override Guid MangaGuid { get { return Mintmanga.Type; } }
    public override Type MangaType { get { return typeof(Mintmanga); } }
    public override Type LoginType { get { return typeof(GroupleLogin); } }
  }
}