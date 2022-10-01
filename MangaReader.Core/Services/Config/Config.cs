namespace MangaReader.Core.Services.Config
{
  public class Config
  {
    /// <summary>
    /// Настройки программы.
    /// </summary>
    public AppConfig AppConfig { get; set; }

    /// <summary>
    /// Настройки внешнего вида.
    /// </summary>
    public ViewConfig ViewConfig { get; set; }


    public Config()
    {
      this.AppConfig = new AppConfig();
      this.ViewConfig = new ViewConfig();
    }
  }
}
