using System.Collections.Concurrent;
using System.Threading;

namespace MangaReader.Core.Services
{
  public static class CallContext<T>
  {
    private static readonly ConcurrentDictionary<string, AsyncLocal<T>> State = new ConcurrentDictionary<string, AsyncLocal<T>>();

    public static void SetData(string name, T data)
    {
      State.AddOrUpdate(name, new AsyncLocal<T>(), (_, __) => new AsyncLocal<T>()).Value = data;
    }

    public static T GetData(string name)
    {
      return State.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : default(T);
    }

    public static bool RemoveData(string name)
    {
      return State.TryRemove(name, out _);
    }
  }
}
