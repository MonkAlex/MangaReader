using MangaReader.Core.Manga;

namespace MangaReader.Core.Exception
{
  public class DownloadAttemptFailed : MangaReaderException
  {
    public static string Error = "Load failed after {0} counts";

    public int Count { get; }

    public IDownloadable Downloadable { get; }

    public override string FormatMessage()
    {
      return string.Format("{0}. Download '{1}' to '{2}'.", string.Format(Error, Count), Downloadable.Uri, Downloadable.Folder);
    }

    public DownloadAttemptFailed(int count, IDownloadable downloadable) : base()
    {
      Count = count;
      Downloadable = downloadable;
    }
  }
}