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
    Account.CookieClient GetCookieClient();
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
