using System;
using System.Drawing;
using System.Drawing.Imaging;
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

    /// <summary>
    /// Сжать изображение.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <returns>Сжатое изображение.</returns>
    public static byte[] GetThumbnail(byte[] image)
    {
      var memory = new MemoryStream();
      Image
          .FromStream(new MemoryStream(image))
          .GetThumbnailImage(128, 200, null, new IntPtr())
          .Save(memory, ImageFormat.Png);
      return memory.ToArray();
    }

    /// <summary>
    /// Получить расширение изображения.
    /// </summary>
    /// <param name="image">Изображение.</param>
    /// <returns>Строка с типом картинки.</returns>
    public static string GetImageExtension(byte[] image)
    {
      var created = Image.FromStream(new MemoryStream(image));
      return new ImageFormatConverter().ConvertToString(created.RawFormat).ToLower();
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="url">Ссылка на файл.</param>
    /// <returns>Содержимое файла.</returns>
    public static byte[] DownloadFile(string url)
    {
      Byte[] result;
      WebResponse response;
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
        return null;
      }

      return response.ContentLength == result.LongLength ? result : null;
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
