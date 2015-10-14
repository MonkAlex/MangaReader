using System;
using NHibernate.Id;

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
          throw new IdentifierGenerationException("Нельзя изменять ID сущности.");
      }
      get { return id; }
    }

    private int id = 0;

    protected virtual void BeforeSave(object[] currentState, object[] previousState, string[] propertyNames)
    {

    }

    public virtual void BeforeSave(object[] currentState, object[] previousState,
      string[] propertyNames, NHibernate.Type.IType[] types)
    {
      this.BeforeSave(currentState, previousState, propertyNames);
    }

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    public virtual void Save()
    {
      var session = Mapping.Environment.Session;
      using (var tranc = session.BeginTransaction())
      {
        try
        {
          this.Save(session, tranc);
          tranc.Commit();
          session.Flush();
        }
        catch (Exception)
        {
          tranc.Rollback();
          throw;
        }
      }
    }

    public virtual void Save(NHibernate.ISession session, NHibernate.ITransaction transaction)
    {
      if (this.Id == 0)
        session.Save(this);
      else
        session.Update(this);
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
      Mapping.Environment.Session.Flush();
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
        session.Flush();
        this.Id = 0;
      }
      return true;
    }
  }
}
