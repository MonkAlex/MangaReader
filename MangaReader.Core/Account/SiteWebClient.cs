using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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

    public Uri MainUri { get; }

    public Type PluginType { get; set; }

    public CookieContainer CookieContainer { get; }

    public Task<Page> GetPage(Uri uri)
    {
      var client = GetCookieClient();
      return DoWithRestarts(uri, client, GetPageImpl, new Page(uri));
    }

    private CookieClient GetCookieClient()
    {
      return new CookieClient(this.CookieContainer)
      {
        BaseAddress = MainUri.OriginalString,
        Proxy = MangaSettingCache.Get(PluginType).Proxy,
      };
    }

    private static async Task<Page> GetPageImpl(Uri uri, CookieClient client)
    {
      var task = client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
      return new Page(await task, client.ResponseUri);
    }

    private static async Task<T> DoWithRestarts<T>(Uri uri, CookieClient client,
      Func<Uri, CookieClient, Task<T>> func, T defaultValue, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, uri);

        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          return await func(uri, client).ConfigureAwait(false);
        }
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, $"Некорректная ссылка: {uri}");
        return defaultValue;
      }
      catch (WebException ex)
      {
        Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {uri}, попытка номер - {restartCounter}");
        ++restartCounter;

        if (await ExceptionCanBeRetry(ex).ConfigureAwait(false))
          return await DoWithRestarts(uri, client, func, defaultValue, restartCounter).ConfigureAwait(false);

        return defaultValue;
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить страницу: {uri}");
        return defaultValue;
      }
    }

    public Task<byte[]> GetData(Uri uri)
    {
      var client = GetCookieClient();
      return DoWithRestarts(uri, client, (u, c) => c.DownloadDataTaskAsync(u), null);
    }

    public Task<Page> Post(Uri uri, Dictionary<string, string> parameters)
    {
      var client = GetCookieClient();
      var nvc = new NameValueCollection();
      foreach (var parameter in parameters)
      {
        nvc.Add(parameter.Key, parameter.Value);
      }

      return DoWithRestarts(uri, client, (u, c) => PostImpl(u, c, nvc), new Page(uri));
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

    public SiteWebClient(Uri mainUri, IPlugin plugin, CookieContainer cookieContainer)
    {
      MainUri = mainUri;
      CookieContainer = cookieContainer;
      PluginType = plugin.GetType();
    }
  }
}
