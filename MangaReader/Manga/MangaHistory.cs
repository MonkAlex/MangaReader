using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaReader
{
  // История манги.
  public class MangaHistory : Entity.Entity
  {

    /// <summary>
    /// Ссылка на мангу.
    /// Исключительно для десериализации старых данных.
    /// </summary>
    public virtual string MangaUrl { get; set; }

    /// <summary>
    /// Ссылка в историю.
    /// </summary>
    public virtual string Url { get; set; }

    /// <summary>
    /// Время добавления.
    /// </summary>
    public virtual DateTime Date { get; set; }

    #region Equals

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      var mangaHistory = obj as MangaHistory;
      return mangaHistory != null && this.Url.Equals(mangaHistory.Url);
    }

    public override int GetHashCode()
    {
      return this.Url.GetHashCode();
    }

    #endregion

    /// <summary>
    /// Создать историю из ссылок.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    [Obsolete]
    public static List<MangaHistory> CreateHistories(IEnumerable<string> messages)
    {
      return messages.Select(message => new MangaHistory(message)).ToList();
    }

    public MangaHistory() { }

    public MangaHistory(string message)
    {
      this.Url = message;
      this.Date = DateTime.Now;
    }
  }
}
