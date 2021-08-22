using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MangaReader.Core.Exception;

namespace Hentaichan.Mangachan
{
  public class MangachanChapter : MangaReader.Core.Manga.Chapter
  {
    public int VolumeNumber { get; }

    public MangachanChapter(Uri uri, string name)
      : base(uri, name)
    {
      var match = Regex.Match(name, @"[a-zA-Z_а-яА-Я]+.*?(\d+).*?[a-zA-Z_-а-яА-Я]+.*?(\d+\.\d+|\d+)");
      if (match.Groups.Count > 2)
      {
        VolumeNumber = int.Parse(match.Groups[1].Value);
        Number = double.Parse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
      }
      else
      {
        throw new MangaReaderException($"Not found volume and chapter numbers in {name} ({uri})");
      }
    }

    protected MangachanChapter()
    {

    }
  }
}
