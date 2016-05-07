using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace MangaReader.Core.Services
{
  public class ImageFile
  {
    protected internal virtual byte[] Body { get; set; }

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

    protected internal virtual bool Exist { get { return this.Body != null; } }

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
    public virtual void Save(string path)
    {
      File.WriteAllBytes(path, this.Body);
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
    internal static ImageFile DownloadFile(Uri uri)
    {
      byte[] result;
      WebResponse response;
      var file = new ImageFile();
      var request = WebRequest.Create(uri);

      try
      {
        response = request.GetResponse();
        using (var ms = new MemoryStream())
        {
          response.GetResponseStream().CopyTo(ms);
          result = ms.ToArray();
        }
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
  }
}
