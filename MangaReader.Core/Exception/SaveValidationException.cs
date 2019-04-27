namespace MangaReader.Core.Exception
{
  public class SaveValidationException : EntityException<Manga.IManga>
  {
    public override string FormatMessage()
    {
      return $"{base.FormatMessage()} Manga uri = '{this.Entity.Uri}'.";
    }

    public SaveValidationException(string message, Manga.IManga manga) : base(message, manga)
    {
    }
  }
}
