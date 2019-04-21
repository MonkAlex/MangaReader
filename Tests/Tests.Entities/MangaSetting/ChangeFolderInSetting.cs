using System.IO;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;
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
  }
}
