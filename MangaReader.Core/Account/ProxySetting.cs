using System;
using System.Net;
using MangaReader.Core.Services;

namespace MangaReader.Core.Account
{
  public class ProxySetting : Entity.Entity
  {
    internal static readonly Lazy<IWebProxy> SystemProxy = new Lazy<IWebProxy>(() =>
    {
      var proxy = WebRequest.GetSystemWebProxy();
      proxy.Credentials = CredentialCache.DefaultCredentials;
      return proxy;
    });

    public string Name { get; set; }

    public Uri Address { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public ProxySettingType SettingType { get; set; }

    public virtual IWebProxy GetProxy()
    {
      switch (SettingType)
      {
        case ProxySettingType.NoProxy:
          return null;
        case ProxySettingType.System:
          return SystemProxy.Value;
        case ProxySettingType.Manual:
          return new WebProxy(Address, true, null, new NetworkCredential(UserName, Password));
        case ProxySettingType.Parent:
          return MangaSettingCache.Get(typeof(IPlugin)).Proxy;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    protected ProxySetting()
    {
      this.SettingType = ProxySettingType.NoProxy;
    }

    public ProxySetting(ProxySettingType settingType)
    {
      this.SettingType = settingType;
    }
  }

  public enum ProxySettingType
  {
    NoProxy,
    System,
    Manual,
    Parent
  }
}
