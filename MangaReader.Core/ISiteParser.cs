using System.Collections.Generic;
using System.Threading.Tasks;
using MangaReader.Core.Manga;

namespace MangaReader.Core
{
  public interface ISiteParser
  {
    /// <summary>
    /// Обновить имя и статус манги.
    /// </summary>
    /// <param name="manga">Манга.</param>
    Task UpdateNameAndStatus(IManga manga);

    /// <summary>
    /// Обновить служебную информацию - например признак наличия глав и томов в манге.
    /// </summary>
    /// <param name="manga">Манга.</param>
    Task UpdateContentType(IManga manga);

    /// <summary>
    /// Обновить служебную информацию - особенно ту, что нужна для скачивания манги, главы, тома, прочее.
    /// </summary>
    /// <param name="manga"></param>
    Task UpdateContent(IManga manga);

    /// <summary>
    /// Update pages (and other info) in chapter.
    /// </summary>
    /// <param name="chapter">Chapter</param>
    Task UpdatePages(Chapter chapter);

    /// <summary>
    /// Распарсить ссылку на мангу.
    /// </summary>
    /// <param name="uri">Ссылка.</param>
    /// <returns>Результат парсинга.</returns>
    UriParseResult ParseUri(System.Uri uri);

    /// <summary>
    /// Получить превьюшки для манги.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <returns>Превью для манги. Может быть пустым или содержать несколько вариантов превью.</returns>
    Task<IEnumerable<byte[]>> GetPreviews(IManga manga);

    /// <summary>
    /// Искать мангу на сайте.
    /// </summary>
    /// <param name="name">Название манги.</param>
    /// <returns>Найденная манга.</returns>
    IAsyncEnumerable<IManga> Search(string name);
  }

  public class UriParseResult
  {
    public bool CanBeParsed { get; }

    public UriParseKind Kind { get; }

    public System.Uri MangaUri { get; }

    public UriParseResult(bool parsed, UriParseKind kind, System.Uri uri)
    {
      this.CanBeParsed = parsed;
      this.Kind = kind;
      this.MangaUri = uri;
    }
  }

  public enum UriParseKind
  {
    Manga = 0,
    Volume = 1,
    Chapter = 2,
    Page = 3
  }
}