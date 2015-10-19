using System.Windows;
using MangaReader.UI.MainForm;

namespace MangaReader.Services.Config
{
  public class ViewConfig
  {
    public WindowStates WindowStates { get; set; }

    public void UpdateWindowState(BaseForm main)
    {
      if (WindowStates == null)
        return;

      main.Top = WindowStates.Top;
      main.Left = WindowStates.Left;
      main.Width = WindowStates.Width;
      main.Height = WindowStates.Height;
      main.WindowState = WindowStates.WindowState;
    }

    public void SaveWindowState(BaseForm main)
    {
      if (WindowStates == null)
        WindowStates = new WindowStates();

      WindowStates.Top = main.Top;
      WindowStates.Left = main.Left;
      WindowStates.Width = main.Width;
      WindowStates.Height = main.Height;
      WindowStates.WindowState = main.WindowState;
    }

    public ViewConfig()
    {
      this.WindowStates = new WindowStates();
    }
  }

  public class WindowStates
  {
    public double Top { get; set; }
    public double Left { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public WindowState WindowState { get; set; }
  }
}