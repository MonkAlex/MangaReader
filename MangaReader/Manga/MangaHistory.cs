using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaReader
{
    // История манги.
    public class MangaHistory
    {
        /// <summary>
        /// Ссылка на мангу.
        /// </summary>
        public string MangaUrl;

        /// <summary>
        /// Ссылка в историю.
        /// </summary>
        public string Url;

        /// <summary>
        /// Время добавления.
        /// </summary>
        public DateTime Date;

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
