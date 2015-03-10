using System;
using System.Net;

namespace MangaReader.Account
{
  public class CookieClient : WebClient
  {

    internal CookieContainer Cookie = new CookieContainer();

    internal Uri ResponseUri;

    protected override WebRequest GetWebRequest(Uri address)
    {
      var request = base.GetWebRequest(address);
      if (request is HttpWebRequest)
        (request as HttpWebRequest).CookieContainer = Cookie;
      return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
      var baseResponse = base.GetWebResponse(request);
      this.ResponseUri = baseResponse.ResponseUri;
      return baseResponse;
    }
  }
}
