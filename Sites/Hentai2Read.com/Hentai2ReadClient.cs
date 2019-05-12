using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentai2Read.com
{
  public class Hentai2ReadClient : CookieClient
  {
    public Hentai2ReadClient()
    {
      Proxy = MangaSettingCache.Get(typeof(Hentai2ReadPlugin)).Proxy;
    }
  }
}
