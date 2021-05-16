using System;
using System.Net;
using System.Text;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  [System.ComponentModel.DesignerCategory("Code")]
  internal sealed class CookieClient : WebClient
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
      return ParseResponseUri(() => base.GetWebResponse(request));
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
      return ParseResponseUri(() => base.GetWebResponse(request, result));
    }

    private WebResponse ParseResponseUri(Func<WebResponse> getResponse)
    {
      WebResponse baseResponse;
      try
      {
        baseResponse = getResponse();
        this.ResponseUri = baseResponse.ResponseUri;
      }
      catch (WebException e)
      {
        baseResponse = e.Response;
        if (baseResponse is HttpWebResponse response)
        {
          if (response.StatusCode != HttpStatusCode.Redirect)
            throw;

          var uriString = response.Headers.Get("Location");
          if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var location))
            this.ResponseUri = location;
          else
            Log.Exception(e, $"Parsing location failed. '{uriString}'");
        }
      }

      return baseResponse;
    }

    public CookieClient(CookieContainer cookie)
    {
      this.Cookie = cookie;
      this.Encoding = Encoding.UTF8;
      Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0";
    }
  }
}
