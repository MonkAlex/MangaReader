using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Markup;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia
{
  public class BitmapTypeConverter : IValueConverter
  {
    public static BitmapTypeConverter Instance => new BitmapTypeConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var array = value as byte[];
      if (array == null)
        return null;

      try
      {
        using (var memory = new MemoryStream(array))
        {
          return new Bitmap(memory);
        }
      }
      catch (Exception e)
      {
        Log.Exception(e);
      }
      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}