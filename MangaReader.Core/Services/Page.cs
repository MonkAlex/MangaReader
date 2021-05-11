using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.Properties;

namespace MangaReader.Core.Services
{
  public class Page
  {
    /// <summary>
    /// 4 minutes delay for 429 error
    /// </summary>
    private const int Delay = 240000;

    public bool HasContent { get { return !string.IsNullOrWhiteSpace(this.Content); } }

    public string Content { get; set; }

    public Uri ResponseUri { get; set; }

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

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static async Task<Page> GetPageAsync(Uri url, Task<CookieClient> client, int restartCounter = 0)
    {
      return await GetPageAsync(url, await client.ConfigureAwait(false), restartCounter).ConfigureAwait(false);
    }

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static async Task<Page> GetPageAsync(Uri url, CookieClient client, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, url);

        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          var task = client.DownloadStringTaskAsync(url).ConfigureAwait(false);
          return new Page(await task, client.ResponseUri);
        }
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, $"Некорректная ссылка: {url}");
        return new Page(url);
      }
      catch (WebException ex)
      {
        Log.Exception(ex, $"{Strings.Page_GetPage_SiteOff}, ссылка: {url}, попытка номер - {restartCounter}");
        ++restartCounter;

        if (await ExceptionCanBeRetry(ex).ConfigureAwait(false))
          return await GetPageAsync(url, client, restartCounter).ConfigureAwait(false);

        return new Page(url);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось получить страницу: {url}");
        return new Page(url);
      }
    }

    public Page()
    {

    }

    public Page(Uri response)
    {
      this.ResponseUri = response;
    }

    public Page(string content, Uri response) : this(response)
    {
      this.Content = content;
    }
  }
}
