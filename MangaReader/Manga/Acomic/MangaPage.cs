using System;
using System.Text.RegularExpressions;

namespace MangaReader.Manga.Acomic
{
  public class MangaPage : MangaReader.Manga.MangaPage
  {
    public MangaPage(Uri uri, Uri imageLink)
      : base(uri, imageLink)
    {
      this.Number = Convert.ToInt32(Regex.Match(uri.OriginalString, @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }
  }
}
