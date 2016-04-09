using System.Collections.Concurrent;

namespace MangaReader.UI.Services
{
  public class SimpleDictionary<TX, TY>
  {
    protected ConcurrentDictionary<TX, TY> Dictionary;

    public virtual bool AddOrReplace(TX key, TY value)
    {
      if (Dictionary.ContainsKey(key))
        Dictionary[key] = value;
      else
        return Dictionary.TryAdd(key, value);
      return true;
    }

    public virtual bool TryRemove(TX key)
    {
      TY value;
      if (Dictionary.ContainsKey(key))
        return Dictionary.TryRemove(key, out value);
      return true;
    }

    public virtual TY TryGet(TX key)
    {
      TY value;
      Dictionary.TryGetValue(key, out value);
      return value;
    }

    public SimpleDictionary()
    {
      Dictionary = new ConcurrentDictionary<TX, TY>();
    }

  }
}