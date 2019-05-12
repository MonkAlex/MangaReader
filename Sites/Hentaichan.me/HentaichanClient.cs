using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public class HentaichanClient : CookieClient
  {
    public HentaichanClient()
    {
      Proxy = MangaSettingCache.Get(typeof(HentaichanPlugin)).Proxy;
    }
  }
}
