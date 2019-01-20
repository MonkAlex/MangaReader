using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class ThrottleService
  {
    private const string ThrottlerName = nameof(IThrottler);

    private static readonly IThrottler DefaultThrottler = new Throttler(15);

    public static Task<IDisposable> WaitAsync()
    {
      return GetThrottler().WaitAsync();
    }

    public static IDisposable Wait()
    {
      return GetThrottler().Wait();
    }

    public static IDisposable SetThrottler(IThrottler throttler)
    {
      CallContext<IThrottler>.SetData(ThrottlerName, throttler);
      return new DisposeAction(() => CallContext<IThrottler>.RemoveData(ThrottlerName));
    }

    private static IThrottler GetThrottler()
    {
      return CallContext<IThrottler>.GetData(ThrottlerName) ?? DefaultThrottler;
    }
  }

  public interface IThrottler
  {
    IDisposable Wait();
    Task<IDisposable> WaitAsync();
  }

  public class Throttler : IThrottler
  {
    private readonly SemaphoreSlim semaphore;

    public Throttler(int maxRequests)
    {
      semaphore = new SemaphoreSlim(maxRequests);
    }

    public IDisposable Wait()
    {
      semaphore.Wait();
      return new DisposeAction(() =>
      {
        semaphore.Release();
      });
    }

    public async Task<IDisposable> WaitAsync()
    {
      await semaphore.WaitAsync();
      return new DisposeAction(() =>
      {
        semaphore.Release();
      });
    }

  }

  internal struct DisposeAction : IDisposable
  {
    private Action action;
    private bool disposed;

    public DisposeAction(Action action)
    {
      this.action = action;
      this.disposed = false;
    }

    public void Dispose()
    {
      if (!disposed)
      {
        this.action();
        disposed = true;
      }
    }
  }

  public static class CallContext<T>
  {
    static ConcurrentDictionary<string, AsyncLocal<T>> state = new ConcurrentDictionary<string, AsyncLocal<T>>();

    public static void SetData(string name, T data) =>
      state.AddOrUpdate(name, new AsyncLocal<T>(), (_, __) => new AsyncLocal<T>()).Value = data;

    public static T GetData(string name) =>
      state.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : default(T);

    public static bool RemoveData(string name) =>
      state.TryRemove(name, out _);
  }
}