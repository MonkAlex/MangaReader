using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Acomics
{
  public class AcomicsClient : CookieClient
  {
    public AcomicsClient()
    {
      Proxy = MangaSettingCache.Get(typeof(AcomicsPlugin)).Proxy;
    }
  }
}
