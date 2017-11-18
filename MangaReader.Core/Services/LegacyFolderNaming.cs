using System;
using System.Globalization;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class LegacyFolderNaming : IFolderNamingStrategy
  {
    public Guid Id { get { return Guid.Parse("D29A161A-8A36-402B-ADEE-7E24724E662F"); } }

    public string Name { get { return "Старый формат (Chapter 001)"; } }

    public string FormateChapterFolder(Chapter chapter)
    {
      return AppConfig.ChapterPrefix + chapter.Number.ToString(CultureInfo.InvariantCulture).PadLeft(chapter.Number % 1 == 0 ? 4 : 6, '0');
    }

    public string FormateVolumeFolder(Volume volume)
    {
      return AppConfig.VolumePrefix + volume.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0');
    }
  }

  public class NameWithNumberPrefixFolderNaming : IFolderNamingStrategy
  {
    public Guid Id { get { return Guid.Parse("FA565D64-FEBA-4C0A-9BCD-E95C784E6329"); } }

    public string Name { get { return "Ведущая цифра и название главы (001 Глава...)"; } }

    public string FormateChapterFolder(Chapter chapter)
    {
      var name = string.IsNullOrWhiteSpace(chapter.Name) ? AppConfig.ChapterPrefix : chapter.Name;
      return chapter.Number.ToString(CultureInfo.InvariantCulture).PadLeft(chapter.Number % 1 == 0 ? 4 : 6, '0') + name;
    }

    public string FormateVolumeFolder(Volume volume)
    {
      var name = string.IsNullOrWhiteSpace(volume.Name) ? AppConfig.VolumePrefix : volume.Name;
      return volume.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0') + name;
    }
  }
}