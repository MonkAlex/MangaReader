using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hentaichan
{
  public class HentaichanChapter : MangaReader.Core.Manga.Chapter
  {
    protected override void UpdatePages()
    {
      Parser.UpdatePages(this);
      base.UpdatePages();
    }

    internal static double GetChapterNumber(Uri uri)
    {
      return double.Parse(Regex.Match(uri.OriginalString, @"/*(\d+\.\d+|\d+)", RegexOptions.RightToLeft).Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    public HentaichanChapter(Uri uri, string desc) 
      : this(uri)
    {
      this.Name = desc;
    }

    public HentaichanChapter(Uri uri) 
      : base(uri)
    {
      this.Number = GetChapterNumber(uri);
    }

    protected HentaichanChapter()
    {
      
    }
  }
}
