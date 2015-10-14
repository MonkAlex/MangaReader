using System;
using System.Text.RegularExpressions;

namespace MangaReader.Manga.Hentaichan
{
  public class MangaPage : MangaReader.Manga.MangaPage
  {
    public MangaPage(Uri uri, Uri imageLink) : base(uri, imageLink)
    {
      this.Number = Convert.ToInt32(Regex.Match(imageLink.OriginalString, @"/*([0-9]+)", RegexOptions.RightToLeft).Groups[1].Value);
    }
  }
}
