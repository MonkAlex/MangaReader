using MangaReader.Core.Exception;
using MangaReader.Mapping;

namespace MangaReader.Entity
{
  public class Entity
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

    protected virtual void BeforeSave(object[] currentState, object[] previousState, string[] propertyNames)
    {

    }

    internal virtual void BeforeSave(object[] currentState, object[] previousState,
      string[] propertyNames, NHibernate.Type.IType[] types)
    {
      this.BeforeSave(currentState, previousState, propertyNames);
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

      Mapping.Environment.Session.Refresh(this);
    }

    /// <summary>
    /// Удалить из базы. Сохранение такой сущности создаст новую в базе.
    /// </summary>
    /// <returns>False, если сущности в базе ещё не было.</returns>
    public virtual bool Delete()
    {
      if (this.Id == 0)
        return false;

      var session = Mapping.Environment.Session;
      using (var tranc = session.BeginTransaction())
      {
        var entity = session.Load(this.GetType(), this.Id);
        session.Delete(entity);
        tranc.Commit();
        this.Id = 0;
      }
      return true;
    }
  }
}
