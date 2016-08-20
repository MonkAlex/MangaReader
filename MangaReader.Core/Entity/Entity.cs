using System.Xml.Serialization;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Entity
{
  public class Entity : IEntity
  {
    [XmlIgnore]
    public int Id
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

    protected virtual void BeforeSave(object[] currentState, object[] previousState, string[] propertyNames)
    {

    }

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    public virtual void Save()
    {
      Repository.Save(this);
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

      Mapping.Session.Refresh(this);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    /// <returns>False, если сущности в базе ещё не было.</returns>
    public virtual bool Delete()
    {
      if (this.Id == 0)
        return false;

      var session = Mapping.Session;
      using (var tranc = session.BeginTransaction())
      {
        var entity = session.Load(this.GetType(), this.Id);
        session.Delete(entity);
        tranc.Commit();
        this.Id = 0;
      }
      return true;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      var entity = obj as Entity;
      if (entity == null)
        return false;

      return Equals(this.Id, entity.Id) && this.GetType() == entity.GetType();
    }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode()^this.GetType().GetHashCode();
    }
  }
}
