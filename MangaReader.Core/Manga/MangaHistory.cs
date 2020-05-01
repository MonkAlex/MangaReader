using System;
using System.Diagnostics;

namespace MangaReader.Core.Manga
{
  // История манги.
  [DebuggerDisplay("Id = {Id}, Uri = {Uri}, Date = {Date}")]
  public class MangaHistory : Entity.Entity
  {

    /// <summary>
    /// Ссылка в историю.
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// Время добавления.
    /// </summary>
    public DateTime Date { get; set; }

    #region Equals

    public override bool Equals(object obj)
    {
      if (obj == null || this.Uri == null)
        return false;

      var mangaHistory = obj as MangaHistory;
      return mangaHistory != null && this.Uri.Equals(mangaHistory.Uri);
    }

    public override int GetHashCode()
    {
      return this.Uri.GetHashCode();
    }

    #endregion

    protected MangaHistory()
    {
      this.Date = DateTime.Now;
    }

    public MangaHistory(Uri message) : this()
    {
      this.Uri = message;
    }
  }
}
