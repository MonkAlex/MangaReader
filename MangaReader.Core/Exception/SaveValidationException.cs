namespace MangaReader.Core.Exception
{
  public class SaveValidationException : MangaReaderException
  {
    public Entity.Entity Entity { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Type '{1}' with id {2}.", base.FormatMessage(), this.Entity.GetType().Name, this.Entity.Id);
    }

    public SaveValidationException(string message, Entity.Entity entity) : base(message)
    {
      this.Entity = entity;
    }
  }
}