namespace MangaReader.Core
{
  public interface IPlugin
  {
    System.Reflection.Assembly Assembly { get; }

    System.Guid MangaGuid { get; }

    System.Type MangaType { get; }

    System.Guid LoginGuid { get; }

    MangaReader.Core.Account.Login GetLogin();

    MangaReader.Core.Services.MangaSetting GetSettings();
  }
}