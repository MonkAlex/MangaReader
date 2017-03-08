using System;
using System.Collections.Generic;
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
      : this(uri)
    {
      this.Name = desc;
    }

    public Chapter(Uri uri)
      : base(uri)
    {
      var match = Regex.Match(uri.OriginalString, @"v(\d+)_ch(\d+\.\d+|\d+)", RegexOptions.RightToLeft);
      if (match.Groups.Count > 2)
      {
        Volume = int.Parse(match.Groups[1].Value);
        Number = int.Parse(match.Groups[2].Value.Replace(".", string.Empty));
      }
    }
  }
}
