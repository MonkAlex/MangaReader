using System;
using System.Net;

namespace MangaReader.Core.Account
{
  public class SiteHttpClientFactory
  {
    public static ISiteHttpClient Get(Uri mainUri, IWebProxy proxy, CookieContainer cookieContainer)
    {
#if NETFRAMEWORK
      var client = new SiteWebClient(mainUri, proxy, cookieContainer);
#else
      var client = new SiteHttpClient(mainUri, proxy, cookieContainer);
#endif

      return client;
    }
  }
}
