using System;
using MangaReader.Core.Manga;

namespace MangaReader.Core.Exception
{
  public class DownloadAttemptFailed : MangaReaderException
  {
    public static string Error = "Load failed after {0} counts";

    public int Count { get; }

    public IDownloadable Downloadable { get; }

    public string Folder { get; }

    public Uri Uri { get; }

    public override string FormatMessage()
    {
      return string.Format("{0}. Download '{1}' to '{2}'.", string.Format(Error, Count), Uri, Folder);
    }

    public DownloadAttemptFailed(int count, IDownloadable downloadable) : this(count, downloadable.Uri)
    {
      Downloadable = downloadable;
      Folder = downloadable.Folder;
    }

    public DownloadAttemptFailed(int count, Uri uri) : base()
    {
      Count = count;
      Uri = uri;
      Folder = "memory";
    }
  }
}