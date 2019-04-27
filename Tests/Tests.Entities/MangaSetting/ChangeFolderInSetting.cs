using System.IO;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  [TestFixture]
  public class ChangeFolderInSetting : TestClass
  {
    [Test]
    public async Task ChangeFolderInSettingNoConflict()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await Builder.CreateAcomics().ConfigureAwait(false);
        var folder = manga.Folder;
        var settingFolder = manga.Setting.Folder;
        Assert.NotNull(folder);
        manga.Setting.Folder += "2";
        Directory.CreateDirectory(manga.Setting.Folder);
        await context.Save(manga.Setting).ConfigureAwait(false);

        Assert.AreNotEqual(folder, manga.Folder);
        await context.Refresh(manga).ConfigureAwait(false);
        Assert.AreNotEqual(folder, manga.Folder);

        manga.Setting.Folder = settingFolder;
        await context.Save(manga.Setting).ConfigureAwait(false);

        Assert.AreEqual(folder, manga.Folder);
      }
    }

    [Test]
    public async Task TestMangaRename([Values(true, false)] bool originalDirectoryExists, [Values(true, false)] bool destinationDirectoryExists)
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await Builder.CreateAcomics().ConfigureAwait(false);

        // settingfolder/manganame
        var folder = manga.Folder;
        if (originalDirectoryExists)
          Directory.CreateDirectory(manga.GetAbsoluteFolderPath());

        manga.Name += "2";
        // settingfolder/manganame2
        var changedFolder = folder + "2";
        Assert.AreNotEqual(folder, changedFolder);

        if (destinationDirectoryExists)
          Directory.CreateDirectory(DirectoryHelpers.GetAbsoluteFolderPath(changedFolder));

        Assert.AreEqual(destinationDirectoryExists, Directory.Exists(DirectoryHelpers.GetAbsoluteFolderPath(changedFolder)));

        async Task SaveManga()
        {
          await context.Save(manga).ConfigureAwait(false);
        }

        if (originalDirectoryExists && destinationDirectoryExists)
          Assert.ThrowsAsync<MangaDirectoryExists>(SaveManga);
        else
          await SaveManga();

        Assert.AreEqual(originalDirectoryExists || destinationDirectoryExists, Directory.Exists(DirectoryHelpers.GetAbsoluteFolderPath(changedFolder)));
      }
    }
  }
}
