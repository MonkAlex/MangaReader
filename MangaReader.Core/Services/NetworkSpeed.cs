using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class NetworkSpeed
  {
    public static double TotalSpeed { get { return totalSpeed; } }

    private static double totalSpeed = 0;

    private const uint Seconds = 3;

    private const uint TimerInterval = 1000;

    private static Timer speedTimer = new Timer(state =>
    {
      var now = 0L;
      while (receivedStorage.Value.Any())
      {
        long added;
        if (receivedStorage.Value.TryDequeue(out added))
        {
          now += added;
        }
      }
      lastSpeeds.Value.Enqueue(now);
      totalSpeed = lastSpeeds.Value.Average();
      OnUpdated(totalSpeed);
    }, null, 0, TimerInterval);

    private static Lazy<LimitedConcurrentQueue<double>> lastSpeeds = new Lazy<LimitedConcurrentQueue<double>>(() => new LimitedConcurrentQueue<double>(Seconds));

    private static Lazy<ConcurrentQueue<long>> receivedStorage = new Lazy<ConcurrentQueue<long>>();

    public static void Clear()
    {
      while (receivedStorage.Value.Count > 0)
      {
        long dd;
        receivedStorage.Value.TryDequeue(out dd);
      }
      while (lastSpeeds.Value.Count > 0)
      {
        double dd;
        lastSpeeds.Value.TryDequeue(out dd);
      }
    }

    public static void AddInfo(long received)
    {
      receivedStorage.Value.Enqueue(received);
    }

    public static event Action<double> Updated; 

    private class LimitedConcurrentQueue<T> : ConcurrentQueue<T>
    {
      public uint Limit { get; }

      public new void Enqueue(T item)
      {
        while (Count >= Limit)
        {
          T deleted;
          TryDequeue(out deleted);
        }
        base.Enqueue(item);
      }

      public LimitedConcurrentQueue(uint limit)
      {
        Limit = limit;
      }
    }

    private static void OnUpdated(double obj)
    {
      Updated?.Invoke(obj);
    }
  }
}
