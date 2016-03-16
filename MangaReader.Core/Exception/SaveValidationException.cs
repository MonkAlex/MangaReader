namespace MangaReader.Core.Exception
{
  public class SaveValidationException : EntityException
  {
    public SaveValidationException(string message, Entity.Entity entity) : base(message, entity)
    {
    }
  }
}