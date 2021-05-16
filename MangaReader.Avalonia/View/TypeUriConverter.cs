using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MangaReader.Avalonia.View
{
  public class UriConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType == typeof(string) && value is Uri uri)
        return uri.OriginalString;

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string uriString && targetType == typeof(Uri))
      {
        if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
        {
          return new Uri(uriString, UriKind.Absolute);
        }

        if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
        {
          return new Uri(uriString, UriKind.Relative);
        }

        if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
          return uri;
      }

      return null;
    }
  }
}
