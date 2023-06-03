﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

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
        await Task.Delay(1000).ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="uri">Ссылка на страницу манги.</param>
    /// <param name="settingCache">Настройки сети.</param>
    /// <returns>Содержимое файла.</returns>
    public static async Task<ImageFile> DownloadImage(Uri uri, MangaSettingCache settingCache, string referer)
    {
      byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Referer = referer;
      request.Proxy = settingCache.Proxy;
      request.Accept = "image/webp,*/*";

      try
      {
        response = await request.GetResponseAsync().ConfigureAwait(false);
        result = await CopyTo(response.GetResponseStream()).ConfigureAwait(false);
      }
      catch (System.Exception ex)
      {
        if (!string.IsNullOrEmpty(uri.Query))
        {
          uri = new Uri(uri.GetLeftPart(UriPartial.Path));
          return await DownloadImage(uri, settingCache, referer);
        }
        Log.Exception(ex, string.Format($"Загрузка {uri} не завершена. Использованы настройки прокси {settingCache.SettingType}"));
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
