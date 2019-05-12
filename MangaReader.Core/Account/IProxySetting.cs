using System.Net;

namespace MangaReader.Core.Account
{
  public interface IProxySetting
  {
    IWebProxy GetProxy();
  }
}
