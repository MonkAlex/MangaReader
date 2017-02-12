namespace MangaReader.Core.Exception
{
  public class SaveValidationException : EntityException
  {
    public SaveValidationException(string message, Entity.IEntity entity) : base(message, entity)
    {
    }
  }
}