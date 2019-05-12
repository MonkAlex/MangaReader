using System;
using System.Net;

namespace MangaReader.Core.Account
{
  public class ProxySetting : Entity.Entity, IProxySetting
  {
    public string Name { get; set; }

    public Uri Address { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public virtual IWebProxy GetProxy()
    {
      var proxy = new WebProxy(Address, true, null, new NetworkCredential(UserName, Password));
      return proxy;
    }
  }
}
