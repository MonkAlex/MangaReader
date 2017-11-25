using System;
using System.Text.RegularExpressions;

namespace Grouple
{
  /// <summary>
  /// Глава.
  /// </summary>
  public class GroupleChapter : MangaReader.Core.Manga.Chapter
  {
    #region Свойства

    /// <summary>
    /// Номер тома.
    /// </summary>
    public int VolumeNumber;

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
    /// <param name="name">Описание главы.</param>
    public GroupleChapter(Uri uri, string name)
      : base(uri, name)
    {
      this.VolumeNumber = Convert.ToInt32(Regex.Match(uri.ToString(), @"vol[-]?[0-9]+").Value.Remove(0, 3));
      this.Number = Convert.ToInt32(Regex.Match(uri.ToString(), @"/[-]?[0-9]+", RegexOptions.RightToLeft).Value.Remove(0, 1));
    }

    protected GroupleChapter()
    {
      
    }

    #endregion

  }
}