namespace MangaReader.Avalonia.Platform
{
  public interface ITrayIcon : System.IDisposable
  {
    void SetIcon();

    void ShowBalloon(string text);
  }
}
