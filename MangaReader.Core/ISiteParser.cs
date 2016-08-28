using MangaReader.Core.Manga;

namespace MangaReader.Core
{
  public interface ISiteParser
  {
    void UpdateNameAndStatus(IManga manga);
    void UpdateContentType(IManga manga);
    void UpdateContent(IManga manga);
  }
}