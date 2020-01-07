using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia
{
  public class BitmapTypeConverter : IValueConverter
  {
    public static BitmapTypeConverter Instance => new BitmapTypeConverter();
    public static BitmapTypeConverter Instance => instance;

    private static readonly BitmapTypeConverter instance = new BitmapTypeConverter();
    private static readonly Bitmap DefaultImage = GetNotFoundImage();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var array = value as byte[];
      if (array == null)
        return DefaultImage;

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
      return DefaultImage;
    }

    private static Bitmap GetNotFoundImage()
    {
      return new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(GetNotFoundImageName()));
    }

    private static string GetNotFoundImageName()
    {
      var random = new Random();
      return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames()
        .Where(n => n.StartsWith("MangaReader.Avalonia.Assets.") && n.EndsWith(".jpg"))
        .OrderBy(n => random.Next())
        .FirstOrDefault();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
