using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Markup;
using OmniXaml.TypeConversion;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia
{
  public class BitmapTypeConverter : ITypeConverter, IValueConverter
  {
    public static BitmapTypeConverter Instance => new BitmapTypeConverter();

    public bool CanConvertFrom(IValueContext context, Type sourceType)
    {
      return sourceType == typeof(byte[]);
    }

    public bool CanConvertTo(IValueContext context, Type destinationType)
    {
      return false;
    }

    public object ConvertFrom(IValueContext context, CultureInfo culture, object value)
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

    public object ConvertTo(IValueContext context, CultureInfo culture, object value, Type destinationType)
    {
      throw new NotImplementedException();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertFrom(null, culture, value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}