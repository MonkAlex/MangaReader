using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MangaReader.Core.Account
{
  public interface ISiteHttpClient
  {
    Task<Services.Page> GetPage(Uri uri);
    Task<byte[]> GetData(Uri uri);
    Task<Services.Page> Post(Uri uri, Dictionary<string, string> parameters);

    void AddCookie(string name, string value);
    string GetCookie(string name);
    IEnumerable<System.Net.Cookie> GetCookies();
  }
}
