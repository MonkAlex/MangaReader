using System;
using System.Net;
using System.Text;

namespace MangaReader.Core.Account
{
  [System.ComponentModel.DesignerCategory("Code")]
  public class CookieClient : WebClient
  {
    public CookieContainer Cookie { get; set; }

    public Uri ResponseUri;

    private static readonly Lazy<IWebProxy> SystemProxy = new Lazy<IWebProxy>(() =>
    {
      var proxy = WebRequest.GetSystemWebProxy();
      proxy.Credentials = CredentialCache.DefaultCredentials;
      return proxy;
    });

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

    public CookieClient() : this(new CookieContainer())
    {

    }

    public CookieClient(CookieContainer cookie)
    {
      this.Encoding = Encoding.UTF8;
      this.Proxy = SystemProxy.Value;
      this.Cookie = cookie;
      Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (compatible; MangaReader/1.0)";
    }
  }
}
