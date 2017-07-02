using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests.Entities.Compression
{
  [TestFixture]
  public class ArchiveNotInclude
  {
    [Test]
    public void ChapterArchiveNotIncludeInVolumeArchive()
    {
      var downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "ArchiveNotInclude");
      if (Directory.Exists(downloadFolder))
        Directory.Delete(downloadFolder, true);
      Directory.CreateDirectory(downloadFolder);

      var directory = AddFile(downloadFolder);
      AddFile(directory);
      MangaReader.Core.Services.Compression.CompressChapters(downloadFolder);
      CheckFiles(downloadFolder, 1, 1);
      MangaReader.Core.Services.Compression.CompressVolumes(downloadFolder);
      CheckFiles(downloadFolder, 2, 1, 1);
    }

    private string AddFile(string downloadFolder, string subFolder = null)
    {
      var directory = Path.Combine(downloadFolder, Guid.NewGuid().ToString("D"));
      Directory.CreateDirectory(subFolder ?? directory);

      var path = Path.Combine(subFolder ?? directory, Guid.NewGuid().ToString("D") + ".txt");
      File.Create(path).Dispose();

      return directory;
    }

    private void CheckFiles(string downloadFolder, int archiveCount, params int[] filesCount)
    {
      var files = Directory.GetFiles(downloadFolder, "*", SearchOption.AllDirectories);
      Assert.AreEqual(files.Count(f => f.EndsWith(MangaReader.Core.Services.Compression.ArchiveFormat)), archiveCount);

      var archives = files.Where(n => n.Contains(MangaReader.Core.Services.Compression.ArchiveFormat));
      var count = 0;
      foreach (var archive in archives)
      {
        using (var zip = ZipFile.Open(archive, ZipArchiveMode.Update, Encoding.UTF8))
        {
          Assert.AreEqual(zip.Entries.Count, filesCount[count++]);
        }
      }
    }
  }
}