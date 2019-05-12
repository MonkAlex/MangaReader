using System;
using System.Net;

namespace MangaReader.Core.Account
{
  public class SystemProxySetting : IProxySetting
  {
    internal static readonly Lazy<IWebProxy> SystemProxy = new Lazy<IWebProxy>(() =>
    {
      var proxy = WebRequest.GetSystemWebProxy();
      proxy.Credentials = CredentialCache.DefaultCredentials;
      return proxy;
    });

    public IWebProxy GetProxy()
    {
      return SystemProxy.Value;
    }
  }
}