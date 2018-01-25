using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class DownloadManager
  {
    public static bool IsPaused { get; set; }

    public static async Task CheckPause()
    {
      if (!IsPaused)
        return;

      while (IsPaused)
      {
        await Task.Delay(1000);
      }      
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="uri">Ссылка на файл.</param>
    /// <returns>Содержимое файла.</returns>
    public static async Task<ImageFile> DownloadImage(Uri uri)
    {
      byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Referer = uri.Host;

      try
      {
        response = await request.GetResponseAsync();
        result = await CopyTo(response.GetResponseStream());
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, string.Format("Загрузка {0} не завершена.", uri));
        return file;
      }
      if (response.ContentLength <= result.LongLength)
        file.Body = result;
      return file;
    }

    private static async Task<byte[]> CopyTo(Stream from)
    {
      using (var memory = new MemoryStream())
      {
        byte[] buffer = new byte[81920];
        while (true)
        {
          int num = await from.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
          if (num != 0)
          {
            memory.Write(buffer, 0, num);
            NetworkSpeed.AddInfo(num);
          }
          else
          {
            break;
          }
        }
        return memory.ToArray();
      }
    }
  }
}
