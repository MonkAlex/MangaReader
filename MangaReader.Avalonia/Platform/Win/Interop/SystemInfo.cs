using Avalonia.Controls.ApplicationLifetimes;

namespace MangaReader.Avalonia.Platform.Win.Interop
{
  /// <summary>
  /// This class is a helper for system information, currently to get the DPI factors
  /// </summary>
  public static class SystemInfo
  {
    static SystemInfo()
    {
      double scaling = 1;
      if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
      {
        if (lifetime.MainWindow != null)
        {
          scaling = lifetime.MainWindow.PlatformImpl.Scaling;
        }
      }
      DpiFactorX = scaling;
      DpiFactorY = scaling;
    }

    /// <summary>
    /// Returns the DPI X Factor
    /// </summary>
    public static double DpiFactorX;

    /// <summary>
    /// Returns the DPI Y Factor
    /// </summary>
    public static double DpiFactorY;
  }
}
