namespace MangaReader.Core.Exception
{
  public class MangaDirectoryExists : SaveValidationException
  {
    public string Directory { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Адрес: '{1}'.", base.FormatMessage(), Directory);
    }

    public MangaDirectoryExists(string message, string directory, Entity.Entity entity) : base(message, entity)
    {
      this.Directory = directory;
    }
  }
}