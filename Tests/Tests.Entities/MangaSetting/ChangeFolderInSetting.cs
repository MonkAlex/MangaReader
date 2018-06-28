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
      using (Repository.GetEntityContext())
      {
        var manga = Builder.CreateAcomics();
        var folder = manga.Folder;
        var settingFolder = manga.Setting.Folder;
        Assert.NotNull(folder);
        manga.Setting.Folder += "2";
        manga.Setting.Save();

        Assert.AreNotEqual(folder, manga.Folder);
        manga.Update();
        Assert.AreNotEqual(folder, manga.Folder);

        manga.Setting.Folder = settingFolder;
        manga.Setting.Save();

        Assert.AreEqual(folder, manga.Folder);
      }
    }
  }
}