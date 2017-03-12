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

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент, если нужен специфичный.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static Page GetPage(Uri url, CookieClient client = null, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new DownloadAttemptFailed(restartCounter, url);

        var webClient = client ?? new CookieClient();
        var content = webClient.DownloadString(url);
        return new Page(content, webClient.ResponseUri);
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, "Некорректная ссылка:", url.ToString());
        return new Page();
      }
      catch (WebException ex)
      {
        Library.Status = Strings.Page_GetPage_InternetOff;
        Log.Exception(ex, Strings.Page_GetPage_InternetOff + ", ссылка:" + url + $"restart count - {restartCounter}");

        if (ex.Status != WebExceptionStatus.Timeout && !DelayOnExpectationFailed(ex))
          return new Page();
        ++restartCounter;
        return GetPage(url, client, restartCounter);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, ", ссылка:", url.ToString());
        return new Page();
      }
    }

    public static bool DelayOnExpectationFailed(WebException ex)
    {
      var response = ex.Response as HttpWebResponse;
      if (response != null && response.StatusCode == HttpStatusCode.ExpectationFailed)
      {
        Log.Exception(ex, $"Failed, 417. {response.ResponseUri}");
        var delay = new TimeSpan(0, 4, 0);
        Task.Delay(delay).Wait();
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

        var webClient = client ?? new CookieClient();
        var task = webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);
        return new Page(await task, webClient.ResponseUri);
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, "Некорректная ссылка:", url.ToString());
        return new Page();
      }
      catch (WebException ex)
      {
        Library.Status = Strings.Page_GetPage_InternetOff;
        Log.Exception(ex, Strings.Page_GetPage_InternetOff, ", ссылка:", url.ToString());
        if (ex.Status != WebExceptionStatus.Timeout)
          return new Page();
        ++restartCounter;
        return await GetPageAsync(url, client, restartCounter).ConfigureAwait(false);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, ", ссылка:", url.ToString());
        return new Page();
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
