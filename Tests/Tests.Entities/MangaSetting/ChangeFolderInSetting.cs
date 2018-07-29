using MangaReader.Core.NHibernate;
using NUnit.Framework;

namespace Tests.Entities.MangaSetting
{
  [TestFixture]
  public class ChangeFolderInSetting : TestClass
  {
    [Test]
    public void ChangeFolderInSettingNoConflict()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manga = Builder.CreateAcomics();
        var folder = manga.Folder;
        var settingFolder = manga.Setting.Folder;
        Assert.NotNull(folder);
        manga.Setting.Folder += "2";
        context.Save(manga.Setting);

        Assert.AreNotEqual(folder, manga.Folder);
        context.Refresh(manga);
        Assert.AreNotEqual(folder, manga.Folder);

        manga.Setting.Folder = settingFolder;
        context.Save(manga.Setting);

        Assert.AreEqual(folder, manga.Folder);
      }
    }
  }
}