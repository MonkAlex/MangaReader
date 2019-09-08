using System.Runtime.InteropServices;
using MangaReader.Avalonia.Platform.Win.Interop;

namespace MangaReader.Avalonia.Platform.Win
{
  public class WindowsTrayIcon : ITrayIcon
  {
    public TaskBarIcon TaskBarIcon;

    public void SetIcon()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        var iconSource = "MangaReader.Avalonia.Assets.main.ico";
        var icon = TaskBarIcon.ToIcon(iconSource);
        TaskBarIcon = new TaskBarIcon(icon);
      }
    }

    public void ShowBalloon(string text)
    {
      TaskBarIcon?.ShowBalloonTip(nameof(MangaReader), text, BalloonFlags.Info);
    }

    public void Dispose()
    {
      TaskBarIcon?.Dispose();
    }
  }
}
