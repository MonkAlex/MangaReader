using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangaReader.Core.Services
{
  public class ThrottleService : IDisposable
  {
    private static SemaphoreSlim throttler = new SemaphoreSlim(15);

    private bool? released = null;

    public static async Task<IDisposable> WaitAsync()
    {
      await throttler.WaitAsync();
      return new ThrottleService();
    }

    public static IDisposable Wait()
    {
      throttler.Wait();
      return new ThrottleService();
    }

    private ThrottleService()
    {
      released = false;
    }

    public void Dispose()
    {
      if (released == false)
      {
        throttler.Release();
        released = true;
      }
    }
  }
}