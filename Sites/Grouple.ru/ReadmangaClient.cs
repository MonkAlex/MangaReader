using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Grouple
{
  public class ReadmangaClient : CookieClient
  {
    public ReadmangaClient()
    {
      this.Proxy = MangaSettingCache.Get(typeof(ReadmangaPlugin)).Proxy;
    }
  }
}
