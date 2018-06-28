using System.Xml.Serialization;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Entity
{
  public class Entity : IEntity
  {
    [XmlIgnore]
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

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    public virtual void Save()
    {
      Repository.Save(this);
    }

    public virtual void BeforeSave(ChangeTrackerArgs args)
    {

    }

    /// <summary>
    ///  Загрузить свежую информацию из базы.
    /// </summary>
    public virtual void Update()
    {
      if (this.Id == 0)
      {
        // TODO: надо бы обнулить всю сущность.
        return;
      }

      using (var context = Repository.GetEntityContext())
        context.Refresh(this);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    /// <returns>False, если сущности в базе ещё не было.</returns>
    public virtual bool Delete()
    {
      if (this.Id == 0)
        return false;

      using (var context = Repository.GetEntityContext())
      {
        using (var tranc = context.OpenTransaction())
        {
          context.Delete(this);
          tranc.Commit();
          this.Id = 0;
        }
      }
      return true;
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
