using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Entities.Compression
{
  [TestClass]
  public class MangaCompressionDeleteFolders
  {
    [TestMethod]
    public void MangaFolderIsEmptyAfterCompression()
    {
      var downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "MangaCompressionDeleteFolders");
      if (Directory.Exists(downloadFolder))
        Directory.Delete(downloadFolder, true);
      Directory.CreateDirectory(downloadFolder);

      var volume = AddFolder(downloadFolder);
      var chapter = AddFolder(volume);
      AddFile(chapter);
      MangaReader.Core.Services.Compression.CompressManga(downloadFolder);
      CheckFiles(downloadFolder, 1, 1);

      Assert.IsFalse(Directory.Exists(chapter));
      Assert.IsFalse(Directory.Exists(volume));
      Assert.IsTrue(Directory.Exists(downloadFolder));
    }

    private string AddFolder(string downloadFolder)
    {
      var directory = Path.Combine(downloadFolder, Guid.NewGuid().ToString("D"));
      Directory.CreateDirectory(directory);
      return directory;
    }

    private void AddFile(string folder)
    {
      var file = Path.Combine(folder, Guid.NewGuid().ToString("D"));
      File.Create(file).Dispose();
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