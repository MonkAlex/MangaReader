namespace MangaReader.Avalonia.Platform
{
  public interface ITrayIcon : System.IDisposable
  {
    System.Windows.Input.ICommand DoubleClickCommand { get; set; }

    void SetIcon();

    void ShowBalloon(string text);
  }
}
