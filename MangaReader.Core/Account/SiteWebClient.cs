using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  public class SiteWebClient : ISiteHttpClient
  {
    /// <summary>
    /// 4 minutes delay for 429 error
    /// </summary>
    private const int Delay = 240000;

    private readonly Uri mainUri;

    private readonly IWebProxy proxy;

    private readonly CookieContainer cookieContainer;

    public async Task<Page> GetPage(Uri uri)
    {
      var client = GetCookieClient();
      var (page, exception) = await DoWithRestarts(uri, client, GetPageImpl).ConfigureAwait(false);
      return page ?? new Page(uri) { Error = exception.GetBaseException().Message };
    }

    private CookieClient GetCookieClient()
    {
      return new CookieClient(this.cookieContainer)
      {
        BaseAddress = mainUri.OriginalString,
        Proxy = proxy,
      };
    }

    private static async Task<Page> GetPageImpl(Uri uri, CookieClient client)
    {
      var task = client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
      return new Page(await task, client.ResponseUri);
    }

    private static async Task<(T, System.Exception)> DoWithRestarts<T>(Uri uri, CookieClient client,
      Func<Uri, CookieClient, Task<T>> func, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, uri);

        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          var value = await func(uri, client).ConfigureAwait(false);
          return (value, null);
        }
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, $"Некорректная ссылка: {uri}");
        return (default, ex);
      }
      catch (WebException ex)
      {
        Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {uri}, попытка номер - {restartCounter}");
        ++restartCounter;

        if (await ExceptionCanBeRetry(ex).ConfigureAwait(false))
          return await DoWithRestarts(uri, client, func, restartCounter).ConfigureAwait(false);

        return (default, ex);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить страницу: {uri}");
        return (default, ex);
      }
    }

    public async Task<byte[]> GetData(Uri uri)
    {
      var client = GetCookieClient();
      var (data, _) = await DoWithRestarts(uri, client, (u, c) => c.DownloadDataTaskAsync(u)).ConfigureAwait(false);

      return data;
    }

    public Task<Page> Post(Uri uri, Dictionary<string, string> parameters)
    {
      return Post(uri, parameters, null);
    }

    public async Task<Page> Post(Uri uri, Dictionary<string, string> parameters, Dictionary<string, string> headers)
    {
      var client = GetCookieClient();
      var nvc = new NameValueCollection();
      foreach (var parameter in parameters)
      {
        nvc.Add(parameter.Key, parameter.Value);
      }
      if (headers != null)
      {
        foreach (var header in headers)
        {
          client.Headers.Add(header.Key, header.Value);
        }
      }

      var (page, exception) = await DoWithRestarts(uri, client, (u, c) => PostImpl(u, c, nvc)).ConfigureAwait(false);
      return page ?? new Page(uri) { Error = exception.GetBaseException().Message };
    }

    public void AddCookie(string name, string value)
    {
      cookieContainer.Add(new Cookie(name, value, "/", mainUri.Host)
      {
        Expires = DateTime.Today.AddYears(1)
      });
    }

    public string GetCookie(string name)
    {
      return GetCookies()
        .Where(c => c.Name == name)
        .Select(c => c.Value)
        .Distinct()
        .Single();
    }

    public IEnumerable<Cookie> GetCookies()
    {
      return cookieContainer.GetCookies(this.mainUri)
        .Cast<Cookie>();
    }

    public static async Task<bool> DelayOnExpectationFailed(WebException ex)
    {
      if (ex.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.ExpectationFailed || response.StatusCode == (HttpStatusCode)429))
      {
        Log.Error($"Доступ к {response.ResponseUri} будет повторно проверен через 4 минуты.");
        await Task.Delay(Delay).ConfigureAwait(false);
        return true;
      }
      return false;
    }

    private static async Task<Page> PostImpl(Uri uri, CookieClient client, NameValueCollection nvc)
    {
      var response = await client.UploadValuesTaskAsync(uri, nvc).ConfigureAwait(false);
      return new Page(Encoding.UTF8.GetString(response), client.ResponseUri);
    }

    private static async Task<bool> ExceptionCanBeRetry(WebException ex)
    {
      if (ex.Status == WebExceptionStatus.Timeout)
        return true;

      if (await DelayOnExpectationFailed(ex).ConfigureAwait(false))
        return true;

      if (ex.HResult == -2146893023)
        return true;

      if (ex.HResult == 2147012721)
        return true;

      if (ex.InnerException is IOException)
        return true;

      if (ex.InnerException is SocketException)
        return true;

      if (ex.Response is HttpWebResponse httpWebResponse)
      {
        if (httpWebResponse.StatusCode == HttpStatusCode.InternalServerError)
          return true;
        if (httpWebResponse.StatusCode == HttpStatusCode.BadGateway)
          return true;
        if (httpWebResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
          return true;
        if (httpWebResponse.StatusCode == HttpStatusCode.GatewayTimeout)
          return true;
      }

      return false;
    }

    public SiteWebClient(Uri mainUri, IWebProxy proxy, CookieContainer cookieContainer)
    {
      this.mainUri = mainUri;
      this.cookieContainer = cookieContainer;
      this.proxy = proxy;
    }
  }
}
