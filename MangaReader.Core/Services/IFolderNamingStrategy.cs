using MangaReader.Core.Manga;

namespace MangaReader.Core.Services
{
  public interface IFolderNamingStrategy
  {
    System.Guid Id { get; }
    string Name { get; }

    string FormateChapterFolder(Chapter chapter);
    string FormateVolumeFolder(Volume volume);
  }
}