namespace MangaReader.Core.Exception
{
  public class EntityException : MangaReaderException
  {
    public Entity.IEntity Entity { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Type '{1}' with id {2}.", base.FormatMessage(), this.Entity.GetType().Name, this.Entity.Id);
    }

    public EntityException(string message, Entity.IEntity entity) : base(message)
    {
      this.Entity = entity;
    }
  }
}