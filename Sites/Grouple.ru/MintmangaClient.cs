using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace Grouple
{
  public class MintmangaClient : CookieClient
  {
    public MintmangaClient()
    {
      this.Proxy = MangaSettingCache.Get(typeof(MintmangaPlugin)).Proxy;
    }
  }
}