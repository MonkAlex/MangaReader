using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing.Imaging;

namespace MangaReader.Core.Services
{
  public class ImageFile
  {
    public virtual byte[] Body { get; set; }

    protected internal virtual bool Exist
    {
      get { return this.Body != null && this.Body.LongLength > 0; }
    }

    protected internal virtual string Extension
    {
      get
      {
        if (this.Exist && string.IsNullOrWhiteSpace(this.extension))
        {
          var created = Image.FromStream(new MemoryStream(this.Body));
          var parsed = ImageFileExtension(created.RawFormat, "jpg");
          this.extension = parsed.ToLower();
        }

        return this.extension;
      }
    }

    private string extension = string.Empty;

    public virtual string Path { get; set; }

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
        await sourceStream.WriteAsync(this.Body, 0, this.Body.Length).ConfigureAwait(false);
      }

      this.Path = path;
    }

    // Code reworked from https://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/Advanced/ImageFormatConverter.cs
    private static string ImageFileExtension(ImageFormat value, string defaultValue)
    {
      var props = typeof(ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public);
      foreach (var p in props)
      {
        if (p.GetValue(null, null).Equals(value))
          return p.Name;
      }

      return defaultValue;
    }
  }
}
