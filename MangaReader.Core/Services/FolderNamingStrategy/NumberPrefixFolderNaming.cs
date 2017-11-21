using System;
using System.Globalization;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class NumberPrefixFolderNaming : IFolderNamingStrategy
  {
    public Guid Id { get { return Guid.Parse("1F7DD841-BF7B-4CFB-A149-C6B085119CC2"); } }

    public string Name { get { return "Ведущая цифра (001 Chapter)"; } }

    public string FormateChapterFolder(Chapter chapter)
    {
      return chapter.Number.ToString(CultureInfo.InvariantCulture).PadLeft(chapter.Number % 1 == 0 ? 4 : 6, '0') + " " + AppConfig.ChapterPrefix;
    }

    public string FormateVolumeFolder(Volume volume)
    {
      return volume.Number.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0') + " " + AppConfig.VolumePrefix;
    }
  }
}