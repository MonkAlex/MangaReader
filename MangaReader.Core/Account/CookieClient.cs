using System;
using System.Net;
using System.Text;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  [System.ComponentModel.DesignerCategory("Code")]
  public class CookieClient : WebClient
  {
    public CookieContainer Cookie { get; set; }

    public Uri ResponseUri;

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

    protected CookieClient()
    {
      this.Cookie = new CookieContainer();
      this.Encoding = Encoding.UTF8;
      Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
    }
  }
}
