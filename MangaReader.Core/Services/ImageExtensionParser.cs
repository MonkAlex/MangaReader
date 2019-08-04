using System;
using System.Linq;

namespace MangaReader.Core.Services
{
  /// <summary>
  /// Класс для анализа формата изображения по его содержимому.
  /// </summary>
  public class ImageExtensionParser
  {
    public const string Jpeg = "jpeg";
    public const string Png = "png";
    public const string Gif = "gif";
    public const string Bmp = "bmp";
    public const string Tiff = "tiff";
    private static readonly byte[] bmp = { 0x42, 0x4D };
    private static readonly byte[] gif = { 0x47, 0x49, 0x46, 0x38 };
    private static readonly byte[] jpg = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] png = { 0x89, 0x50, 0x4E, 0x47 };
    private static readonly byte[] tiff1 = { 0x49, 0x49, 0x2A, 0x00 };
    private static readonly byte[] tiff2 = { 0x4D, 0x4D, 0x00, 0x2A };

    /// <summary>
    /// Спарсить содержимое.
    /// </summary>
    /// <param name="value">Тело изображения.</param>
    /// <param name="defaultValue">Значение по умолчанию, если спарсить не удалось.</param>
    /// <returns>Формат изображения в нижнем регистре.</returns>
    public static string Parse(byte[] value, string defaultValue)
    {
      if (value.LongLength < 4)
        return defaultValue;

      var head = new byte[4];
      Array.Copy(value, head, 4);

      if (jpg.SequenceEqual(head.Take(jpg.Length)))
        return Jpeg;
      if (png.SequenceEqual(head.Take(png.Length)))
        return Png;
      if (gif.SequenceEqual(head.Take(gif.Length)))
        return Gif;
      if (bmp.SequenceEqual(head.Take(bmp.Length)))
        return Bmp;
      if (tiff1.SequenceEqual(head.Take(tiff1.Length)))
        return Tiff;
      if (tiff2.SequenceEqual(head.Take(tiff2.Length)))
        return Tiff;

      var headAsString = string.Join(" ", head.Select(b => $"0x{b:x2}"));
      Log.Add($"Unknown file format with head {headAsString}");

      return defaultValue;
    }
  }
}
