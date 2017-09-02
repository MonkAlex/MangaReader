using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hentaichan.Mangachan
{
  public class Chapter : MangaReader.Core.Manga.Chapter
  {
    public int Volume { get; }

    protected override void UpdatePages()
    {
      Parser.UpdatePages(this);
      base.UpdatePages();
    }

    public Chapter(Uri uri, string desc)
      : base(uri)
    {
      this.Name = desc;
      var match = Regex.Match(Name, @"v(\d+) - (\d+\.\d+|\d+)", RegexOptions.RightToLeft);
      if (match.Groups.Count > 2)
      {
        Volume = int.Parse(match.Groups[1].Value);
        Number = double.Parse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
      }
    }
  }
}
