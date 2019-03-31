using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.API
{
  [TestFixture]
  class FolderNameNormalization
  {
    [Test, Sequential]
    public void CheckNotAllowedNames([Values(
      "test"
      ,@"C:\manga\manga:name"
      ,@"usr\home\manga:name"
      ,@"start < > : "" / \ | ? * end"
      ,"\x15\x3D" // less than ASCII space
      ,"\x21\x3D" // HEX of !, valid
      ,"\x3F\x3D" // HEX of ?, not valid
      ,@"C:\manga\   trailing space   "
      ,@"C:\manga\...trailing period..."
      ,@"C:\manga\CON"
      ,@"C:\manga\CON.txt"
      ,@"CON"
      ,@"C:\manga\con.txt\context"
      ,@"home\NUL.liza"
      ,@"home\ NUL.liza"
      ,@"root\..\sub"
      ,@"root\..\"
      ,@".\..\some?folder"
      ,@"C:\manga\..." // Bad manga name get the root folder, bug =_=
      ,@"root\.." // relative path trimmed, bug =_=
    )] string name, [Values(
      "test"
      ,@"C:\manga\manga.name"
      ,@"usr\home\manga.name"
      ,@"start . . . . . \ . . . end"
      ,".="
      ,"!="
      ,".="
      ,@"C:\manga\   trailing space"
      ,@"C:\manga\...trailing period"
      ,@"C:\manga\.CON"
      ,@"C:\manga\.CON.txt"
      ,@".CON"
      ,@"C:\manga\.CON.txt\context"
      ,@"home\.NUL.liza"
      ,@"home\ NUL.liza"
      ,@"root\..\sub"
      ,@"root\..\"
      ,@".\..\some.folder"
      ,@"C:\manga\"
      ,@"root\"
    )] string expected)
    {
      Assert.AreEqual(expected, DirectoryHelpers.MakeValidPath(name));
    }
  }
}