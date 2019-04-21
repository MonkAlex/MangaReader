using System.IO;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.API
{
  [TestFixture]
  class FolderNameNormalization
  {
    [Test, Sequential]
    public void CheckNotAllowedMangaNames([Values(
        "test"
        , @"manga:name"
        , @"start < > : "" / \ | ? * end"
        , "\x15\x3D" // less than ASCII space
        , "\x21\x3D" // HEX of !, valid
        , "\x3F\x3D" // HEX of ?, not valid
        , @"   trailing space   "
        , @"...trailing period..."
        , @"CON"
        , @"CON.txt"
        , @"context"
        , @"NUL.liza"
        , @" NUL.liza"
        , @"some?folder"
        , @"..."
        , @".."
      )]
      string name, [Values(
        "test"
        , @"manga.name"
        , @"start . . . . . . . . . end"
        , ".="
        , "!="
        , ".="
        , @"   trailing space"
        , @"...trailing period"
        , @".CON"
        , @".CON.txt"
        , @"context"
        , @".NUL.liza"
        , @" NUL.liza"
        , @"some.folder"
        , @"invalid name"
        , @"invalid name"
      )]
      string expected)
    {
      Assert.AreEqual(expected, DirectoryHelpers.RemoveInvalidCharsFromName(name));
    }

    [Test, Sequential]
    public void CheckNotAllowedSettingFolders([Values(
        "test"
        , @"manga\manga.name"
        , @"root\..\sub"
        , @"root\..\"
        , @"root\.."
        , @".\..\"
        , @".\.."
        , @".\"
        , @".\..\some.folder"
      )]
      string name, [Values(
        true
        , true
        , true
        , false
        , false
        , false
        , false
        , false
        , true
      )]
      bool allowed)
    {
      var path = Path.Combine(TestContext.CurrentContext.TestDirectory, name);
      Directory.CreateDirectory(path);
      Assert.AreEqual(allowed, DirectoryHelpers.ValidateSettingPath(path));
    }
  }
}
