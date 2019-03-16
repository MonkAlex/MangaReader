using System.Threading.Tasks;
using MangaReader.Core.Exception;

namespace MangaReader.Core.Entity
{
  public class Entity : IEntity
  {
    public virtual int Id
    {
      set
      {
        if (id == 0 || value == 0)
          id = value;
        else
          throw new EntityException("Нельзя изменять ID сущности.", this);
      }
      get { return id; }
    }

    private int id = 0;
    
    public virtual Task BeforeSave(ChangeTrackerArgs args)
    {
      return Task.CompletedTask;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      if (!(obj is Entity entity))
        return false;

      return Equals(this.Id, entity.Id) && this.GetType() == entity.GetType();
    }

    public override int GetHashCode()
    {
      if (this.Id == 0)
        return base.GetHashCode()^this.GetType().GetHashCode();
      return this.Id.GetHashCode()^this.GetType().GetHashCode();
    }
  }
}
