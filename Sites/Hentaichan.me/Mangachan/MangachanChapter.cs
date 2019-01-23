using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hentaichan.Mangachan
{
  public class MangachanChapter : MangaReader.Core.Manga.Chapter
  {
    public int VolumeNumber { get; }

    public MangachanChapter(Uri uri, string name)
      : base(uri, name)
    {
      var match = Regex.Match(name, @"v(\d+) - (\d+\.\d+|\d+)", RegexOptions.RightToLeft);
      if (match.Groups.Count > 2)
      {
        VolumeNumber = int.Parse(match.Groups[1].Value);
        Number = double.Parse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
      }
    }

    protected MangachanChapter()
    {
      
    }
  }
}
