using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MangaReader.Avalonia.ViewModel.Explorer;

namespace MangaReader.Avalonia.View
{
  public class DownloadColorConverter : IValueConverter
  {
    public static DownloadColorConverter Instance => new DownloadColorConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is MangaModel manga)
      {
        if (manga.Downloaded == 100)
          return Brushes.GreenYellow;
      }

      return Brushes.Yellow;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
