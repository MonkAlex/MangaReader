using System;
using System.Text.RegularExpressions;

namespace MangaReader.Core.Manga.Hentaichan
{
  public class Chapter : Core.Manga.Chapter
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
      var fromUri = Regex.Match(uri.OriginalString, @"/*(\d+\.\d+|\d+)", RegexOptions.RightToLeft)
        .Groups[1].Value.Replace(".", string.Empty);
      this.Number = Convert.ToInt32(fromUri);
    }
  }
}
