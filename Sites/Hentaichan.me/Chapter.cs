using System;
using System.Text.RegularExpressions;

namespace Hentaichan
{
  public class Chapter : MangaReader.Core.Manga.Chapter
  {
    protected override void UpdatePages()
    {
      Parser.UpdatePages(this);
      base.UpdatePages();
    }

    internal static double GetChapterNumber(Uri uri)
    {
      return double.Parse(Regex.Match(uri.OriginalString, @"/*(\d+\.\d+|\d+)", RegexOptions.RightToLeft).Groups[1].Value);
    }

    public Chapter(Uri uri, string desc) 
      : this(uri)
    {
      this.Name = desc;
    }

    public Chapter(Uri uri) 
      : base(uri)
    {
      this.Number = GetChapterNumber(uri);
    }
  }
}
