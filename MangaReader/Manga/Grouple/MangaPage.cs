using System;

namespace MangaReader.Manga.Grouple
{
  public class MangaPage : MangaReader.Manga.MangaPage
  {
    public MangaPage(Uri uri, Uri imageLink, int number) : base(uri, imageLink)
    {
      this.Number = number;
    }
  }
}
