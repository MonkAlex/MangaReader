using System;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Services
{
  [TestFixture, Parallelizable(ParallelScope.All)]
  public class FolderNaming
  {
    private Chapter chapter = new Grouple.GroupleChapter(new Uri("https://readmanga.live/tower_of_god/vol2/318"), "Башня Бога 2 - 318 44F Конечная станция (03)");
    private Volume volume = new Volume("Башня бога 2", 1);

    [Test, Sequential]
    public void ByName([Values(
        typeof(LegacyFolderNaming),
        typeof(NumberPrefixFolderNaming),
        typeof(NameWithNumberPrefixFolderNaming))]
      Type strategyType,
      [Values(
        "Chapter 0318",
        "0318 Chapter",
        "0318 Башня Бога 2 - 318 44F Конечная станция (03)")]
      string chapterName,
      [Values(
        "Volume 001",
        "001 Volume",
        "001 Башня бога 2")]
      string volumeName)
    {
      var strategy = Activator.CreateInstance(strategyType) as IFolderNamingStrategy;
      var chapterFolder = strategy.FormateChapterFolder(chapter);
      var volumeFolder = strategy.FormateVolumeFolder(volume);
      Assert.AreEqual(chapterName, chapterFolder);
      Assert.AreEqual(volumeName, volumeFolder);
      Assert.AreNotEqual(Guid.Empty, strategy.Id);
    }
  }
}
