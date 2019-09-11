using System.Runtime.InteropServices;
using System.Windows.Input;
using MangaReader.Avalonia.Platform.Win.Interop;

namespace MangaReader.Avalonia.Platform.Win
{
  public class WindowsTrayIcon : ITrayIcon
  {
    public ICommand DoubleClickCommand { get; set; }

    private TaskBarIcon taskBarIcon;

    public void SetIcon()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        var iconSource = "MangaReader.Avalonia.Assets.main.ico";
        var icon = TaskBarIcon.ToIcon(iconSource);
        taskBarIcon = new TaskBarIcon(icon);
        taskBarIcon.MouseEventHandler += TaskBarIconOnMouseEventHandler;
      }
    }

    public void ShowBalloon(string text)
    {
      taskBarIcon?.ShowBalloonTip(nameof(MangaReader), text, BalloonFlags.Info);
    }

    private void TaskBarIconOnMouseEventHandler(object sender, MouseEvent e)
    {
      if (e == MouseEvent.IconDoubleClick)
      {
        var command = this.DoubleClickCommand;
        if (command != null && command.CanExecute(null))
        {
          command.Execute(null);
        }
      }
    }

    public void Dispose()
    {
      if (taskBarIcon != null)
        taskBarIcon.MouseEventHandler -= TaskBarIconOnMouseEventHandler;
      taskBarIcon?.Dispose();
    }
  }
}
