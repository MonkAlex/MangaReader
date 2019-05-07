using System;
using System.IO;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  [TestFixture]
  public class ChangeFolderInSetting : TestClass
  {
    // Test AddToTransaction
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
        Directory.CreateDirectory(DirectoryHelpers.GetAbsoluteFolderPath(manga.Setting.Folder));
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
          await SaveManga().ConfigureAwait(false);

        Assert.AreEqual(originalDirectoryExists || destinationDirectoryExists, Directory.Exists(DirectoryHelpers.GetAbsoluteFolderPath(changedFolder)));
      }
    }

    [Test]
    [Parallelizable(ParallelScope.None)]
    public async Task ChangeFolderInSettingWithManga([Values] SettingDirectory newSettingPath)
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = await Builder.CreateAcomics().ConfigureAwait(false);
        var mangaFolder = manga.GetAbsoluteFolderPath();
        Directory.CreateDirectory(mangaFolder);

        string settingPath;
        switch (newSettingPath)
        {
          case SettingDirectory.Same:
            settingPath = "Download1";
            break;
          case SettingDirectory.Subfolder:
            settingPath = @"Download2\Subfolder3";
            break;
          case SettingDirectory.Parent:
            settingPath = "Download4";
            break;
          case SettingDirectory.AnotherMangaFolder:
            settingPath = (await Builder.CreateAcomics().ConfigureAwait(false)).Folder;
            break;
          case SettingDirectory.AnotherMangaAnotherTypeFolder:
            settingPath = (await Builder.CreateReadmanga().ConfigureAwait(false)).Folder;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(newSettingPath), newSettingPath, null);
        }

        var setting = manga.Setting;
        var oldSettingFolder = DirectoryHelpers.GetAbsoluteFolderPath(setting.Folder);
        setting.Folder = settingPath;
        Directory.CreateDirectory(DirectoryHelpers.GetAbsoluteFolderPath(setting.Folder));
        var newSettingFolder = DirectoryHelpers.GetAbsoluteFolderPath(setting.Folder);

        async Task SaveMangaSetting()
        {
          await context.Save(setting).ConfigureAwait(false);
        }

        if (newSettingPath > SettingDirectory.Parent)
          Assert.ThrowsAsync<MangaSettingSaveValidationException>(SaveMangaSetting);
        else
        {
          await SaveMangaSetting().ConfigureAwait(false);

          // Manga folder must be moved
          var newMangaFolder = manga.GetAbsoluteFolderPath();
          Assert.AreNotEqual(mangaFolder, newMangaFolder);
          Assert.IsTrue(Directory.Exists(newMangaFolder));
          Assert.IsFalse(Directory.Exists(mangaFolder));

          // Setting folders must be exists
          Assert.IsTrue(Directory.Exists(oldSettingFolder));
          Assert.IsTrue(Directory.Exists(newSettingFolder));
        }
      }
    }

    public enum SettingDirectory
    {
      Same,
      Subfolder,
      Parent,
      AnotherMangaFolder,
      AnotherMangaAnotherTypeFolder
    }
  }
}
