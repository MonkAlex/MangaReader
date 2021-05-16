using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  public class LoginCache
  {
    private static Dictionary<Guid, ILogin> logins = new Dictionary<Guid, ILogin>();

    public static void Reset()
    {
      if (logins.Any())
      {
        logins.Clear();
        Log.Add($"LoginCache reset.");
      }
    }

    private static async Task Refresh()
    {
      logins = (await Repository.GetStatelessAsync<MangaSetting>().ConfigureAwait(false))
        .Select(m => new { m.Manga, m.Login })
        .ToDictionary(k => k.Manga, v => v.Login);
      Log.Add($"LoginCache refresh.");
    }

    public static async Task<ILogin> Get(Guid guid)
    {
      if (!logins.Any())
        await Refresh().ConfigureAwait(false);

      return logins[guid];
    }
  }
}
