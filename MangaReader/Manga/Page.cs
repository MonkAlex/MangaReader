using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader
{
  public static class Page
  {
    internal static bool CopyDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        if (!Directory.Exists(destFolder))
          Directory.CreateDirectory(destFolder);
        var files = Directory.GetFiles(sourceFolder);
        foreach (var file in files)
        {
          var name = Path.GetFileName(file);
          var dest = Path.Combine(destFolder, name);
          File.Copy(file, dest);
        }
        var folders = Directory.GetDirectories(sourceFolder);
        foreach (var folder in folders)
        {
          var name = Path.GetFileName(folder);
          var dest = Path.Combine(destFolder, name);
          if (!CopyDirectory(folder, dest))
            return false;
        }
        return true;
      }
      catch (Exception e)
      {
        Log.Exception(e, string.Format("Не удалось скопировать {0} в {1}.", sourceFolder, destFolder));
        return false;
      }
    }

    internal static bool MoveDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        if (CopyDirectory(sourceFolder, destFolder))
          Directory.Delete(sourceFolder, true);
        else
          throw new Exception(string.Format("Не удалось скопировать {0} в {1}.", sourceFolder, destFolder));
        return true;
      }
      catch (Exception ex)
      {
        Log.Exception(ex, string.Format("Не удалось переместить {0} в {1}.", sourceFolder, destFolder));
        return false;
      }
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="url">Ссылка на файл.</param>
    /// <returns>Содержимое файла.</returns>
    internal static ImageFile DownloadFile(string url)
    {
      Byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = WebRequest.Create(url);

      try
      {
        response = request.GetResponse();
        var ms = new MemoryStream();
        response.GetResponseStream().CopyTo(ms);
        result = ms.ToArray();
        ms.Dispose();
      }
      catch
      {
        return file;
      }
      if (response.ContentLength == result.LongLength)
        file.Body = result;
      return file;
    }

    /// <summary>
    /// Получить текст страницы.
    /// </summary>
    /// <param name="url">Ссылка на страницу.</param>
    /// <param name="client">Клиент, если нужен специфичный.</param>
    /// <param name="restartCounter">Попыток скачивания.</param>
    /// <returns>Исходный код страницы.</returns>
    public static string GetPage(string url, WebClient client = null, int restartCounter = 0)
    {
      try
      {
        if (restartCounter > 3)
          throw new Exception(string.Format("Load failed after {0} counts.", restartCounter));

        var webClient = client ?? new WebClient { Encoding = Encoding.UTF8 };
        return webClient.DownloadString(new Uri(url));
      }
      catch (UriFormatException ex)
      {
        Log.Exception(ex, "Некорректная ссылка:", url);
        return string.Empty;
      }
      catch (WebException ex)
      {
        Library.Status = Strings.Page_GetPage_InternetOff;
        Log.Exception(ex, Strings.Page_GetPage_InternetOff, ", ссылка:", url);
        if (ex.Status != WebExceptionStatus.Timeout)
          return string.Empty;
        ++restartCounter;
        return GetPage(url, client, restartCounter);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, ", ссылка:", url);
        return string.Empty;
      }
    }

    /// <summary>
    /// Очистка пути от недопустимых символов.
    /// </summary>
    /// <param name="name">Путь.</param>
    /// <returns>Исправленный путь.</returns>
    public static string MakeValidPath(string name)
    {
      const string replacement = ".";
      var matchesCount = Regex.Matches(name, @":\\").Count;
      string correctName;
      if (matchesCount > 0)
      {
        var regex = new Regex(@":", RegexOptions.RightToLeft);
        correctName = regex.Replace(name, replacement, regex.Matches(name).Count - matchesCount);
      }
      else
        correctName = name.Replace(":", replacement);

      var invalidChars = Regex.Escape(string.Concat(new string(Path.GetInvalidPathChars()), "?", "/", "*"));
      var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

      return Regex.Replace(correctName, invalidRegStr, replacement);
    }
  }
}
