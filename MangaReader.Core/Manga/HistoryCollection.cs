using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MangaReader.Core.Manga
{
  public class HistoryCollection : IList<MangaHistory>, ICollection, IDisposable
  {
    private IList<MangaHistory> impl;
    private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

    public HistoryCollection()
    {
      impl = new List<MangaHistory>();
    }

    public IEnumerator<MangaHistory> GetEnumerator()
    {
      return impl.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)impl).GetEnumerator();
    }

    public void Add(MangaHistory item)
    {
      lockSlim.EnterWriteLock();
      try
      {
        if (!impl.Contains(item))
          impl.Add(item);
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    public void Clear()
    {
      lockSlim.EnterWriteLock();
      try
      {
        impl.Clear();
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    public bool Contains(MangaHistory item)
    {
      lockSlim.EnterReadLock();
      try
      {
        return impl.Contains(item);
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    public void CopyTo(MangaHistory[] array, int arrayIndex)
    {
      lockSlim.EnterReadLock();
      try
      {
        impl.CopyTo(array, arrayIndex);
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    public bool Remove(MangaHistory item)
    {
      lockSlim.EnterWriteLock();
      try
      {
        return impl.Remove(item);
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    public void CopyTo(Array array, int index)
    {
      lockSlim.EnterReadLock();
      try
      {
        Array.Copy(impl.ToArray(), array, index);
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    public int Count => impl.Count;
    public object SyncRoot => throw new NotImplementedException();
    public bool IsSynchronized => throw new NotImplementedException();

    public bool IsReadOnly => impl.IsReadOnly;

    public void Dispose()
    {
      lockSlim?.Dispose();
    }

    public int IndexOf(MangaHistory item)
    {
      lockSlim.EnterReadLock();
      try
      {
        return impl.IndexOf(item);
      }
      finally
      {
        lockSlim.ExitReadLock();
      }
    }

    public void Insert(int index, MangaHistory item)
    {
      lockSlim.EnterWriteLock();
      try
      {
        impl.Insert(index, item);
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    public void RemoveAt(int index)
    {
      lockSlim.EnterWriteLock();
      try
      {
        impl.RemoveAt(index);
      }
      finally
      {
        lockSlim.ExitWriteLock();
      }
    }

    public MangaHistory this[int index]
    {
      get
      {
        lockSlim.EnterReadLock();
        try
        {
          return impl[index];
        }
        finally
        {
          lockSlim.ExitReadLock();
        }
      }
      set
      {
        lockSlim.EnterWriteLock();
        try
        {
          impl[index] = value;
        }
        finally
        {
          lockSlim.ExitWriteLock();
        }
      }
    }
  }
}
