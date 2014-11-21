using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Manga;

namespace MangaReader
{
  // История манги.
  public class MangaHistory : Entity.Entity
  {
    
    public virtual Mangas Manga { get; set; }

    /// <summary>
    /// Ссылка на мангу.
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
    public static List<MangaHistory> CreateHistories(IEnumerable<string> messages)
    {
      return messages.Select(message => new MangaHistory(message)).ToList();
    }

    public MangaHistory() { }

    public MangaHistory(string message)
    {
      var builder = new UriBuilder(message);
      var mangaLink = string.Concat(builder.Scheme, Uri.SchemeDelimiter, builder.Host, builder.Uri.Segments[0],
          builder.Uri.Segments[1].Replace(builder.Uri.Segments[0], string.Empty));
      MangaUrl = mangaLink;
      Url = message;
      Date = DateTime.Now;
    }
  }
}
