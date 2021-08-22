using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Account;

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
    protected override async Task ConfigureCookieClient(ISiteHttpClient client, ILogin login)
    {
      await base.ConfigureCookieClient(client, login).ConfigureAwait(false);

      if (!client.GetCookies().Any(c => c.Name == "PHPSESSID"))
      {
        await client.GetPage(login.MainUri).ConfigureAwait(false);
      }
    }

    public override ISiteParser GetParser()
    {
      return new Parser();
    }
  }
}
