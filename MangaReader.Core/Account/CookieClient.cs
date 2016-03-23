using System;
using System.Net;
using System.Text;

namespace MangaReader.Account
{
  public class CookieClient : WebClient
  {
    internal CookieContainer Cookie { get; set; }

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

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
      var baseResponce = base.GetWebResponse(request, result);
      this.ResponseUri = baseResponce.ResponseUri;
      return baseResponce;
    }

    public CookieClient()
    {
      this.Cookie = new CookieContainer();
      this.Encoding = Encoding.UTF8;
    }
  }
}
