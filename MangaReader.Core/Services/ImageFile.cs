using System.IO;
using System.Threading.Tasks;

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
          this.extension = ImageExtensionParser.Parse(this.Body, ImageExtensionParser.Jpeg);

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
  }
}
