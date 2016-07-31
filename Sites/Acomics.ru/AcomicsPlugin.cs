using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;
using MangaReader.Core.Account;

namespace Acomics
{
  [Export(typeof(IPlugin))]
  public class AcomicsPlugin : BasePlugin
  {
    public override string ShortName { get { return "AC"; } }

    public override Assembly Assembly { get {return Assembly.GetAssembly(this.GetType());} }
    public override Guid MangaGuid { get { return Acomics.Type; } }
    public override Type MangaType { get { return typeof (Acomics); } }
    public override Guid LoginGuid { get { return AcomicsLogin.Type; } }
    public override Login GetLogin()
    {
      return Login.Get<AcomicsLogin>();
    }
  }
}