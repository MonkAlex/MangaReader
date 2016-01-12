using System;
using System.Net;
using MangaReader.Account;
using MangaReader.Core.Properties;

namespace MangaReader.Services
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
          throw new Exception(string.Format("Load failed after {0} counts.", restartCounter));

        var webClient = client ?? new CookieClient();
        var result = webClient.DownloadString(url);
        return new Page(result, webClient.ResponseUri);
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
        return GetPage(url, client, restartCounter);
      }
      catch (Exception ex)
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
