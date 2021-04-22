using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;

namespace Grouple
{
  public class GroupleMangaPage : MangaPage
  {
    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на страницу.</param>
    /// <param name="imageLink">Ссылка на изображение.</param>
    /// <param name="number">Номер страницы.</param>
    /// <param name="chapter">Глава, которой принадлежит страница.</param>
    public GroupleMangaPage(Uri uri, Uri imageLink, int number, Chapter chapter) : base(uri, imageLink, number, chapter)
    {
      #warning Class not required now, but its mapped to BD.
    }

    protected GroupleMangaPage() : base()
    {

    }

    #endregion
  }
}
