using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;

namespace Hentaichan
{
  [Export(typeof(IPlugin))]
  public class HentaichanPlugin : IPlugin
  {
    public Assembly Assembly { get { return Assembly.GetAssembly(typeof(HentaichanPlugin)); } }
  }
}