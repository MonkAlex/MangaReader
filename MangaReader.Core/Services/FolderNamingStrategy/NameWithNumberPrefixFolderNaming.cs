using System;
using System.Globalization;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class NameWithNumberPrefixFolderNaming : IFolderNamingStrategy
  {
    public Guid Id { get { return Guid.Parse("FA565D64-FEBA-4C0A-9BCD-E95C784E6329"); } }

    public string Name { get { return "Ведущая цифра и название главы (001 Глава...)"; } }

    public string FormateChapterFolder(Chapter chapter)
    {
      var name = string.IsNullOrWhiteSpace(chapter.Name) ? AppConfig.ChapterPrefix : chapter.Name;
      name = DirectoryHelpers.MakeValidPath(name);
      return chapter.Number.ToString(CultureInfo.InvariantCulture).PadLeft(chapter.Number % 1 == 0 ? 4 : 6, '0') + " " + name;
    }

    public string FormateVolumeFolder(Volume volume)
    {
      var name = string.IsNullOrWhiteSpace(volume.Name) ? AppConfig.VolumePrefix : volume.Name;
      name = DirectoryHelpers.MakeValidPath(name);
      return volume.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0') + " " + name;
    }
  }
}