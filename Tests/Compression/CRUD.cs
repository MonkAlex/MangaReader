using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MangaReader.Tests.Compression
{
  [TestClass]
  public class CRUD
  {
    [TestMethod]
    public void ArchiveCreateUpdate()
    {
      var downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Compress");
      if (Directory.Exists(downloadFolder))
        Directory.Delete(downloadFolder, true);
      Directory.CreateDirectory(downloadFolder);

      var directory = AddFile(downloadFolder);
      Services.Compression.CompressVolumes(downloadFolder);
      CheckFiles(downloadFolder, 1);
      AddFile(downloadFolder, directory);
      Services.Compression.CompressVolumes(downloadFolder);
      CheckFiles(downloadFolder, 2);
    }

    private string AddFile(string downloadFolder, string subFolder = null)
    {
      var directory = Path.Combine(downloadFolder, Guid.NewGuid().ToString("D"));
      Directory.CreateDirectory(subFolder ?? directory);

      var path = Path.Combine(subFolder ?? directory, Guid.NewGuid().ToString("D"));
      File.Create(path).Dispose();

      return directory;
    }

    private void CheckFiles(string downloadFolder, int filesCount)
    {
      var files = Directory.GetFiles(downloadFolder);
      Assert.AreEqual(files.Count(f => f.EndsWith(Services.Compression.ArchiveFormat)), 1);

      using (var zip = ZipFile.Open(files.Last(), ZipArchiveMode.Update, Encoding.UTF8))
      {
        Assert.AreEqual(zip.Entries.Count, filesCount);
      }
    }
  }
}