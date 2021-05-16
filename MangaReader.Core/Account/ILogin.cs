namespace MangaReader.Core.Account
{
  public interface ILogin : Entity.IEntity
  {
    string Name { get; set; }

    string Password { get; set; }

    bool CanLogin { get; }

    System.Uri MainUri { get; set; }

    System.Uri LogoutUri { get; }

    System.Uri BookmarksUri { get; }

    bool IsLogined(System.Guid mangaType);

    System.Threading.Tasks.Task<bool> DoLogin(System.Guid mangaType);

    System.Threading.Tasks.Task<bool> Logout(System.Guid mangaType);

    System.Threading.Tasks.Task<System.Collections.Generic.List<Manga.IManga>> GetBookmarks(System.Guid mangaType);

    event System.EventHandler<bool> LoginStateChanged;
  }
}
