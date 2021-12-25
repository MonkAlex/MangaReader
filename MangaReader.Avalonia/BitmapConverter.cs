using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia
{
  public class BitmapTypeConverter : IValueConverter
  {
    public static BitmapTypeConverter Instance { get; } = new BitmapTypeConverter();

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
      var random = new Random();

      var loader = AvaloniaLocator.Current.GetService<IAssetLoader>();
      var assets = loader.GetAssets(new Uri("avares://MangaReader.Avalonia/Assets/"), null);

      var uri = assets.Where(a => a.AbsolutePath.EndsWith(".jpg"))
        .OrderBy(n => random.Next())
        .FirstOrDefault();

      return new Bitmap(loader.Open(uri));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
