using System;
using System.Text.RegularExpressions;

namespace MangaReader.Manga.Hentaichan
{
  public class Chapter : MangaReader.Manga.Chapter
  {
    protected override void UpdatePages()
    {
      Getter.UpdatePages(this);
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
      this.Number = Convert.ToInt32(Regex.Match(uri.OriginalString, @"/*[0-9]+", RegexOptions.RightToLeft).Value);
    }
  }
}
