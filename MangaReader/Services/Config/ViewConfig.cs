using System.Windows;
using MangaReader.UI.MainForm;

namespace MangaReader.Services.Config
{
  public class ViewConfig
  {
    /// <summary>
    /// Размеры основного окна и его положение.
    /// </summary>
    public WindowStates WindowStates { get; set; }

    /// <summary>
    /// Панель фильтрации.
    /// </summary>
    public LibraryFilter LibraryFilter { get; set; }

    public void UpdateWindowState(BaseForm main)
    {
      if (WindowStates == null)
        return;

      main.WindowState = WindowStates.WindowState;
      if (WindowStates.CanShow)
      {
        main.Top = WindowStates.Top;
        main.Left = WindowStates.Left;
        main.Width = WindowStates.Width;
        main.Height = WindowStates.Height;
      }
    }

    public void SaveWindowState(BaseForm main)
    {
      if (WindowStates == null)
        WindowStates = new WindowStates();

      if (main.WindowState != WindowState.Minimized)
      {
        WindowStates.WindowState = main.WindowState;
        WindowStates.Top = main.Top;
        WindowStates.Left = main.Left;
        WindowStates.Width = main.Width;
        WindowStates.Height = main.Height;
      }
    }

    public ViewConfig()
    {
      this.WindowStates = new WindowStates();
      this.LibraryFilter = new LibraryFilter();
    }
  }

  public class WindowStates
  {
    public double Top { get; set; }
    public double Left { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    internal bool CanShow { get { return this.Width != 0 && this.Height != 0; } }
    public WindowState WindowState { get; set; }
  }
}