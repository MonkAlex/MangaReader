using System;
using System.Text.RegularExpressions;

namespace Grouple
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class Chapter : MangaReader.Core.Manga.Chapter
  {
    #region Свойства

    /// <summary>
    /// Номер тома.
    /// </summary>
    public int Volume;

    #endregion

    #region Методы

    /// <summary>
    /// Заполнить хранилище ссылок.
    /// </summary>
    protected override void UpdatePages()
    {
      Parser.UpdatePages(this);
      base.UpdatePages();
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    /// <param name="desc">Описание главы.</param>
    public Chapter(Uri uri, string desc)
      : this(uri)
    {
      this.Name = desc;
    }

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на главу.</param>
    public Chapter(Uri uri)
      : base(uri)
    {
      this.Volume = Convert.ToInt32(Regex.Match(uri.ToString(), @"vol[-]?[0-9]+").Value.Remove(0, 3));
      this.Number = Convert.ToInt32(Regex.Match(uri.ToString(), @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    #endregion

  }
}