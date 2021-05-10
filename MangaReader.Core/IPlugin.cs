using System.Threading.Tasks;
using MangaReader.Core.Account;

namespace MangaReader.Core
{
  public interface IPlugin
  {
    string Name { get; }
    string ShortName { get; }
    System.Reflection.Assembly Assembly { get; }
    System.Guid MangaGuid { get; }
    System.Type MangaType { get; }
    System.Type LoginType { get; }
    Services.MangaSetting GetSettings();
    ISiteParser GetParser();
    Task<CookieClient> GetCookieClient(bool withLogin);
    HistoryType HistoryType { get; }
  }

  [System.Flags]
  public enum HistoryType
  {
    Page,
    Chapter,
    Volume
  }
}
