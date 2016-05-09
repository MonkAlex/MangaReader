namespace MangaReader.Core.Services.Config
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

    /// <summary>
    /// Уникальный идентификатор выбранного скина.
    /// </summary>
    public System.Guid SkinGuid { get; set; }

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
    public bool CanShow { get { return this.Width != 0 && this.Height != 0; } }
    public WindowState WindowState { get; set; }
  }

  public enum WindowState
  {
    Normal,
    Minimized,
    Maximized,
  }
}