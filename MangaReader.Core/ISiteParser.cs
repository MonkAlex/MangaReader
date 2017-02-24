using MangaReader.Core.Manga;

namespace MangaReader.Core
{
  public interface ISiteParser
  {
    void UpdateNameAndStatus(IManga manga);
    void UpdateContentType(IManga manga);
    void UpdateContent(IManga manga);
    UriParseResult ParseUri(System.Uri uri);
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