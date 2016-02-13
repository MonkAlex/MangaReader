namespace MangaReader.Core.Exception
{
  public class CopyDirectoryFailed : MangaReaderException
  {
    public string SourceDirectory { get; }

    public string DestinationDirectory { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Источник: '{1}', назначение: '{2}'.", base.FormatMessage(), SourceDirectory, DestinationDirectory);
    }

    public CopyDirectoryFailed(string message, string source, string destination, System.Exception exception) : base(message, exception)
    {
      this.SourceDirectory = source;
      this.DestinationDirectory = destination;
    }
  }
}