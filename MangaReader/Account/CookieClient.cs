using System;
using System.Net;

namespace MangaReader.Account
{
  public class CookieClient : WebClient
  {

    private CookieContainer cookie = new CookieContainer();

    protected override WebRequest GetWebRequest(Uri address)
    {
      var request = base.GetWebRequest(address);
      if (request is HttpWebRequest)
        (request as HttpWebRequest).CookieContainer = cookie;
      return request;
    }

  }
}
