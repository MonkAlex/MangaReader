using System.IO;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.API
{
  [TestFixture]
  public class BackupTests
  {
    [Test]
    [TestCase(false, true, FileShare.Read)] // Перемещение, позитивный кейс
    [TestCase(true, true, FileShare.Read)] // Копирование и остановка, негативный кейс (заблокирован основной архив)
    [TestCase(true, true, FileShare.None)] // Копирование и остановка, негативный кейс (заблокирован основной архив, даже чтение запрещено)
    [TestCase(true, false, FileShare.Read)] // Копирование и остановка, негативный кейс (заблокирован бэкап основного архива)
    public void BackupWith(bool locked, bool original, FileShare fileShare)
    {
      // Есть текстовый файл (txt), невалидный архив (cbz) и бэкап невалидного архива (cbz.dbak).
      // При попытке упаковки txt должно:
      // - упаковать только его, архивы не трогать
      // - упасть на попытке отрыть cbz, сделать его бэкап, а т.к. стандартное имя занято - сделать его в в dbak1
      // - удалить упакованный файл
      var workFolderName = TestContext.CurrentContext.Random.GetString(10);
      var workFolder = Path.Combine(TestContext.CurrentContext.WorkDirectory, workFolderName);
      Directory.CreateDirectory(workFolder);
      var originalArchive = Path.Combine(workFolder, $"{workFolderName}.cbz");
      File.WriteAllText(originalArchive, "content");
      var backupArchive = Path.Combine(workFolder, $"{workFolderName}.cbz.dbak");
      File.WriteAllText(backupArchive, "content");

      var filePath = Path.Combine(workFolder, $"content.txt");
      File.WriteAllText(filePath, "content");

      if (locked)
        File.Open(original ? originalArchive : backupArchive, FileMode.Open, FileAccess.Read, fileShare);
      
      var compressedFiles = Compression.CompressManga(workFolder);

      // Если файлы не заняты, или заняты только бэкапы - должно работать.
      if (!locked || !original)
      {
        Assert.Contains(filePath, compressedFiles);
        Assert.AreEqual(1, compressedFiles.Count);
        Assert.IsFalse(File.Exists(filePath));
      }
      else
      {
        Assert.IsEmpty(compressedFiles);
        Assert.IsTrue(File.Exists(filePath));
      }

      Assert.IsTrue(File.Exists($"{originalArchive}.dbak"));
      // Если хватает доступа для чтения архива - делаем бэкап.
      Assert.AreEqual(fileShare == FileShare.Read, File.Exists($"{originalArchive}.dbak1"));
    }
  }
}
