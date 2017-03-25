using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    private static Timer speedTimer = new Timer(state =>
    {
      var now = dpLazy.Value.Sum(v => v.Value.GetSpeed());
      qLazy.Value.Enqueue(now);
      totalSpeed = qLazy.Value.Average() * 1.1;
    }, null, 0, 1000);

    private static Lazy<LimitedConcurrentQueue<double>> qLazy = new Lazy<LimitedConcurrentQueue<double>>(() => new LimitedConcurrentQueue<double>(20));

    private static Lazy<ConcurrentDictionary<Uri, DownloadProgress>> dpLazy = new Lazy<ConcurrentDictionary<Uri, DownloadProgress>>();

    public static void AddInfo(Uri uri, long received, long time)
    {
      var dp = new DownloadProgress(received, time);
      dpLazy.Value.AddOrUpdate(uri, dp, (uri1, progress) => progress = dp);
    }

    public static void RemoveInfo(Uri uri)
    {
      DownloadProgress dp;
      dpLazy.Value.TryRemove(uri, out dp);
    }

    private struct DownloadProgress
    {
      public readonly long BytesReceived;

      // public readonly long TotalBytesToReceive;

      public readonly long TimeMs;

      public double GetSpeed()
      {
        var seconds = TimeMs / 1000.0;
        if (seconds > 0.1)
          return BytesReceived / seconds;
        return 0;
      }

      public DownloadProgress(long received, long time)
      {
        this.BytesReceived = received;
        // this.TotalBytesToReceive = total;
        this.TimeMs = time;
      }

      // public int ProgressPercentage { get; set; }
    }

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
  }
}
