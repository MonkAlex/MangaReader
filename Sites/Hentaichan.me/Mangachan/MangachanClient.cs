using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Hentaichan.Mangachan
{
  public class MangachanClient : CookieClient
  {
    public MangachanClient()
    {
      Proxy = MangaSettingCache.Get(typeof(MangachanPlugin)).Proxy;
    }
  }
}
