using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaReader.Core.Account
{
  public interface ISiteHttpClient
  {
    Uri MainUri { get; }
    System.Net.CookieContainer CookieContainer { get; }

    Task<Services.Page> GetPage(Uri uri);
    Task<byte[]> GetData(Uri uri);
    Task<Services.Page> Post(Uri uri, Dictionary<string, string> parameters);
  }
}
