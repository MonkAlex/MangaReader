using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

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

    public virtual string Url
    {
      get { return Uri == null ? null : Uri.ToString(); }
      set { Uri = value == null ? null : new Uri(value); }
    }

    /// <summary>
    /// Ссылка в историю.
    /// </summary>
    [XmlIgnore]
    public virtual Uri Uri { get; set; }

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
      return mangaHistory != null && this.Uri.Equals(mangaHistory.Uri);
    }

    public override int GetHashCode()
    {
      return this.Uri.GetHashCode();
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
      return messages.Select(message => new MangaHistory(new Uri(message))).ToList();
    }

    public MangaHistory() { }

    public MangaHistory(Uri message)
    {
      this.Uri = message;
      this.Date = DateTime.Now;
    }
  }
}
