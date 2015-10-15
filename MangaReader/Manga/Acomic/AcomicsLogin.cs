using System;
using MangaReader.Account;

namespace MangaReader.Manga.Acomic
{
  public class AcomicsLogin : Login
  {
    public new static Guid Type { get { return Guid.Parse("F526CD85-7846-4F32-85A7-C57E3983DFB1"); } }

    public new static Guid Manga { get { return Acomics.Type; } }
  }
}