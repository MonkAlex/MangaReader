using System;
using System.Collections;
using System.Collections.Generic;

namespace MangaReader.Manga
{
  public class MangasComparer : IComparer<Mangas>, IComparer
  {
    public int Compare(object x, object y)
    {
      var xM = x as Mangas;
      var yM = y as Mangas;
      if (xM != null && yM != null)
        return Compare(xM, yM);
      throw new Exception("Can compare only Mangas.");
    }

    public int Compare(Mangas x, Mangas y)
    {
      return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
    }
  }
}
