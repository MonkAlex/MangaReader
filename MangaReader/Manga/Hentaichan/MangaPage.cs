using System;

namespace MangaReader.Manga.Hentaichan
{
  public class MangaPage : MangaReader.Manga.MangaPage
  {
    public MangaPage(Uri uri, Uri imageLink, int number) : base(uri, imageLink)
    {
      this.Number = number;
    }
  }
}
