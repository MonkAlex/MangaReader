namespace MangaReader.Core.Exception
{
  public class MangaSaveValidationException : EntityException<Manga.IManga>
  {
    public override string FormatMessage()
    {
      return $"{base.FormatMessage()} Manga uri = '{this.Entity.Uri}'.";
    }

    public MangaSaveValidationException(string message, Manga.IManga entity) : base(message, entity)
    {
    }
  }

  public class MangaSettingSaveValidationException : EntityException<Services.MangaSetting>
  {
    public override string FormatMessage()
    {
      return $"{base.FormatMessage()} Settings folder = '{this.Entity.Folder}'.";
    }

    public MangaSettingSaveValidationException(string message, Services.MangaSetting entity) : base(message, entity)
    {
    }
  }
}
