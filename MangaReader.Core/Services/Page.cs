using System;
using System.Net;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.Properties;

namespace MangaReader.Core.Services
{
  public class Page
  {
    public bool HasContent { get { return !string.IsNullOrWhiteSpace(this.Content); } }

    public string Content { get; set; }

    public Uri ResponseUri { get; set; }

    public static async Task<bool> DelayOnExpectationFailed(WebException ex)
    {
      var response = ex.Response as HttpWebResponse;
      if (response != null && (response.StatusCode == HttpStatusCode.ExpectationFailed || response.StatusCode == (HttpStatusCode)429))
      {
        Log.Exception(ex, $"Доступ к {response.ResponseUri} будет повторно проверен через 4 минуты.");
        var delay = new TimeSpan(0, 4, 0);
        await Task.Delay(delay).ConfigureAwait(false);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент, если нужен специфичный.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static async Task<Page> GetPageAsync(Uri url, CookieClient client = null, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, url);

        using (await ThrottleService.WaitAsync().ConfigureAwait(false))
        {
          var webClient = client ?? new CookieClient();
          var task = webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);
          return new Page(await task, webClient.ResponseUri);
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
        if (ex.Status != WebExceptionStatus.Timeout && !(await DelayOnExpectationFailed(ex).ConfigureAwait(false)) && ex.HResult != -2146893023 && ex.HResult != 2147012721)
          return new Page(url);
        ++restartCounter;
        return await GetPageAsync(url, client, restartCounter).ConfigureAwait(false);
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
