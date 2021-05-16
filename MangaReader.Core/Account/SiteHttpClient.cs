using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  public class SiteHttpClient : ISiteHttpClient
  {
    /// <summary>
    /// 4 minutes delay for 429 error
    /// </summary>
    private const int Delay = 240000;

    private readonly HttpClient httpClient;
    public Uri MainUri { get; }
    public CookieContainer CookieContainer { get; }
    public async Task<Page> GetPage(Uri uri)
    {
      var content = await DoWithRestarts(uri, httpClient, (u, c) => c.GetAsync(u)).ConfigureAwait(false);
      if (content == null)
        return new Page(uri);

      var body = await content.Content.ReadAsStringAsync().ConfigureAwait(false);
      uri = GetRequestResponseUri(uri, content);
      return new Page(body, uri);
    }

    public async Task<byte[]> GetData(Uri uri)
    {
      var content = await DoWithRestarts(uri, httpClient, (u, c) => c.GetAsync(u)).ConfigureAwait(false);
      if (content == null)
        return null;

      return await content.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }

    public async Task<Page> Post(Uri uri, Dictionary<string, string> parameters)
    {
      var content = await DoWithRestarts(uri, httpClient, (u, c) => c.PostAsync(u, new FormUrlEncodedContent(parameters))).ConfigureAwait(false);
      if (content == null)
        return new Page(uri);

      var body = await content.Content.ReadAsStringAsync().ConfigureAwait(false);
      uri = GetRequestResponseUri(uri, content);
      return new Page(body, uri);
    }

    private static Uri GetRequestResponseUri(Uri uri, HttpResponseMessage content)
    {
      if (content.Headers.Location != null)
        uri = content.Headers.Location;
      else if (content.RequestMessage.RequestUri != null)
        uri = content.RequestMessage.RequestUri;
      return uri;
    }

    private static async Task<bool> RequestCanBeRetry(HttpResponseMessage content)
    {
      if ((int)content.StatusCode >= 200 && (int)content.StatusCode <= 399)
        return false;

      if (content.StatusCode == HttpStatusCode.RequestTimeout)
        return true;

      if (content.StatusCode == HttpStatusCode.ExpectationFailed ||
          content.StatusCode == (HttpStatusCode)429)
      {
        Log.Error($"Доступ к {content.RequestMessage.RequestUri} будет повторно проверен через 4 минуты.");
        await Task.Delay(Delay).ConfigureAwait(false);
        return true;
      }

      if (content.StatusCode == HttpStatusCode.InternalServerError)
        return true;
      if (content.StatusCode == HttpStatusCode.BadGateway)
        return true;
      if (content.StatusCode == HttpStatusCode.ServiceUnavailable)
        return true;
      if (content.StatusCode == HttpStatusCode.GatewayTimeout)
        return true;

      throw new HttpRequestException((int)content.StatusCode + content.ReasonPhrase);
    }

    private static async Task<HttpResponseMessage> DoWithRestarts(Uri uri, HttpClient client,
      Func<Uri, HttpClient, Task<HttpResponseMessage>> func, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, uri);

        HttpResponseMessage content;
        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          content = await func(uri, client).ConfigureAwait(false);
        }

        if (await RequestCanBeRetry(content).ConfigureAwait(false))
        {
          Log.Error($"{Strings.Page_GetPage_SiteOff}, ссылка: {uri}, попытка номер - {restartCounter}");
          ++restartCounter;
          return await DoWithRestarts(uri, client, func, restartCounter).ConfigureAwait(false);
        }

        return content;
      }
      catch (TaskCanceledException ex)
      {
        Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {uri}, попытка номер - {restartCounter}");
        ++restartCounter;
        return await DoWithRestarts(uri, client, func, restartCounter).ConfigureAwait(false);
      }
      catch (HttpRequestException ex) when (ex.InnerException is System.Net.Sockets.SocketException || ex.InnerException is System.IO.IOException)
      {
        Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {uri}, попытка номер - {restartCounter}");
        ++restartCounter;
        return await DoWithRestarts(uri, client, func, restartCounter).ConfigureAwait(false);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить страницу: {uri}");
        return null;
      }
    }

    public SiteHttpClient(Uri mainUri, IPlugin plugin, CookieContainer cookieContainer)
    {
      this.MainUri = mainUri;
      this.CookieContainer = cookieContainer;
      this.httpClient = new HttpClient(new HttpClientHandler()
      {
        CookieContainer = cookieContainer,
        Proxy = MangaSettingCache.Get(plugin.GetType()).Proxy,
      })
      {
        BaseAddress = mainUri
      };
      this.httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
    }
  }
}
