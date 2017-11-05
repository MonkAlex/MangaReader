using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace MangaReader.Core.NHibernate
{
  public class HistoryBugType : IUserCollectionType
  {
    public HistoryBugType()
    {
    }

    public IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister)
    {
      return new PersistentGenericBag<MangaHistory>(session);
    }

    public IPersistentCollection Wrap(ISessionImplementor session, object collection)
    {
      return new PersistentGenericBag<MangaHistory>(session, (IList<MangaHistory>)collection);
    }

    public IEnumerable GetElements(object collection)
    {
      return collection as IEnumerable;
    }

    public bool Contains(object collection, object entity)
    {
      return ((IEnumerable<MangaHistory>)collection).Contains(entity);
    }

    public object IndexOf(object collection, object entity)
    {
      return -1;
    }

    public object ReplaceElements(object original, object target, ICollectionPersister persister, object owner,
      IDictionary copyCache, ISessionImplementor session)
    {
      var targetCollection = (ICollection<MangaHistory>)target;
      var originalCollection = (ICollection<MangaHistory>)original;

      targetCollection.Clear();
      targetCollection.AddRange(originalCollection);

      return targetCollection;
    }

    public object Instantiate(int anticipatedSize)
    {
      return new HistoryCollection();
    }
  }
}