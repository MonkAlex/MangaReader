namespace MangaReader.Core.Exception
{
  public class MangaDirectoryExists : MangaSaveValidationException
  {
    public string Directory { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Адрес: '{1}'.", base.FormatMessage(), Directory);
    }

    public MangaDirectoryExists(string message, string directory, Manga.IManga entity) : base(message, entity)
    {
      this.Directory = directory;
    }
  }
}
