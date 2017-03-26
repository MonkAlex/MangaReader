using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class ImageFile
  {
    protected virtual byte[] Body { get; set; }

    public virtual string Hash
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.hash))
          this.hash = this.GetHashCode();
        return this.hash;
      }
      set { this.hash = value; }
    }

    private string hash = string.Empty;

    protected internal virtual bool Exist
    {
      get { return this.Body != null; }
    }

    protected internal virtual string Extension
    {
      get
      {
        if (this.Exist && string.IsNullOrWhiteSpace(this.extension))
        {
          var created = Image.FromStream(new MemoryStream(this.Body));
          var parsed = new ImageFormatConverter().ConvertToString(created.RawFormat);
          this.extension = (parsed ?? "jpg").ToLower();
        }
        return this.extension;
      }
      set { this.extension = value; }
    }

    private string extension = string.Empty;

    public virtual string Path { get; set; }

    public virtual new string GetHashCode()
    {
      using (var md5 = MD5.Create())
      {
        return BitConverter.ToString(md5.ComputeHash(md5.ComputeHash(this.Body))).Replace("-", "");
      }
    }

    /// <summary>
    /// Сохранить файл на диск.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    public virtual async Task Save(string path)
    {
      using (FileStream sourceStream = new FileStream(path,
        FileMode.Create, FileAccess.Write, FileShare.None,
        bufferSize: 4096, useAsync: true))
      {
        await sourceStream.WriteAsync(this.Body, 0, this.Body.Length);
      }
      this.Path = path;
    }

    /// <summary>
    /// Удалить файл с диска.
    /// </summary>
    public virtual void Delete()
    {
      if (string.IsNullOrWhiteSpace(this.Path) || !File.Exists(this.Path))
      {
        Log.AddFormat("ImageFile not found. Path - '{0}', Hash - '{1}'.", this.Path, this.Hash);
        return;
      }

      File.Delete(this.Path);
    }

    /// <summary>
    /// Скачать файл.
    /// </summary>
    /// <param name="uri">Ссылка на файл.</param>
    /// <returns>Содержимое файла.</returns>
    public static async Task<ImageFile> DownloadFile(Uri uri)
    {
      byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = WebRequest.Create(uri);

      try
      {
        response = await request.GetResponseAsync();
        result = await CopyTo(response.GetResponseStream(), response.ContentLength, uri);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, string.Format("Загрузка {0} не завершена.", uri));
        return file;
      }
      if (response.ContentLength == result.LongLength)
        file.Body = result;
      return file;
    }

    private static async Task<byte[]> CopyTo(Stream from, long totalBytes, Uri uri)
    {
      var data = new byte[totalBytes];
      byte[] buffer = new byte[81920];
      int currentIndex = 0;
      while (true)
      {
        int num = await from.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        if (num != 0)
        {
          Array.Copy(buffer, 0, data, currentIndex, num);
          currentIndex += num;
          NetworkSpeed.AddInfo(num);
        }
        else
        {
          break;
        }
      }
      return data;
    }
  }
}
