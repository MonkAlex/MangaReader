using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  public class SiteHttpClient : ISiteHttpClient
  {
    private readonly HttpClient httpClient;
    public Uri MainUri { get; }
    public CookieContainer CookieContainer { get; }
    public async Task<Page> GetPage(Uri uri)
    {
      var content = await httpClient.GetAsync(uri).ConfigureAwait(false);
      var body = await content.Content.ReadAsStringAsync().ConfigureAwait(false);
      if (content.Headers.Location != null)
        uri = content.Headers.Location;
      return new Page(body, uri);
    }

    public async Task<byte[]> GetData(Uri uri)
    {
      var content = await httpClient.GetAsync(uri).ConfigureAwait(false);
      return await content.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }

    public async Task<Page> Post(Uri uri, Dictionary<string, string> parameters)
    {
      var content = await httpClient.PostAsync(uri, new FormUrlEncodedContent(parameters)).ConfigureAwait(false);
      var body = await content.Content.ReadAsStringAsync().ConfigureAwait(false);
      if (content.Headers.Location != null)
        uri = content.Headers.Location;
      return new Page(body, uri);
    }

    public SiteHttpClient(Uri mainUri, IPlugin plugin, CookieContainer cookieContainer)
    {
      this.MainUri = mainUri;
      this.CookieContainer = cookieContainer;
      this.httpClient = new HttpClient(new HttpClientHandler()
      {
        CookieContainer = cookieContainer,
        Proxy = MangaSettingCache.Get(plugin.GetType()).Proxy,
      })
      {
        BaseAddress = mainUri
      };
      this.httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
    }
  }
}
