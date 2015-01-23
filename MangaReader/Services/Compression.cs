using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader.Services
{
  class Compression
  {
    private const string ArchiveFormat = ".cbz";
    private const string ArchivePattern = "*.cbz";
    private const string Separator = " ";

    /// <summary>
    /// Упаковка всех глав.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static void CompressChapters(string message)
    {
      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return;

      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        var chapters = Directory.GetDirectories(volume);
        foreach (var chapter in chapters)
        {
          var acr = string.Concat(volume, Path.DirectorySeparatorChar,
              GetFolderName(message), Separator,
              GetFolderName(volume), Separator,
              GetFolderName(chapter), ArchiveFormat);
          if (File.Exists(acr))
            AddToArchive(acr, chapter);
          else
            ZipFile.CreateFromDirectory(chapter, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
          Directory.Delete(chapter, true);
        }
      }
    }

    /// <summary>
    /// Упаковка всех томов.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static void CompressVolumes(string message)
    {
      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return;

      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        var acr = string.Concat(message, GetFolderName(message), Separator, GetFolderName(volume), ArchiveFormat);
        if (File.Exists(acr))
          AddToArchive(acr, volume);
        else
          ZipFile.CreateFromDirectory(volume, acr, CompressionLevel.NoCompression, false, Encoding.UTF8);
        Directory.Delete(volume, true);
      }
    }

    /// <summary>
    /// Упаковка всей манги.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static void CompressManga(string message)
    {
      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return;

      var acr = string.Concat(message, GetFolderName(message), ArchiveFormat);
      var directories = new DirectoryInfo(message);
      var cbzs = directories.GetFiles(ArchivePattern, SearchOption.TopDirectoryOnly);
      var files = directories.GetFiles("*", SearchOption.TopDirectoryOnly);

      if (File.Exists(acr))
        AddToArchive(acr, message);
      else
        using (var fileStream = new FileStream(acr, FileMode.Create))
        {
          using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create, false, Encoding.UTF8))
          {
            foreach (var file in files.Except(cbzs))
            {
              zip.CreateEntryFromFile(file.FullName, file.Name, CompressionLevel.NoCompression);
            }
          }
        }

      var toDelete = directories.GetFiles("*", SearchOption.TopDirectoryOnly).Select(f => f.FullName)
                  .Except(directories.GetFiles(ArchivePattern, SearchOption.TopDirectoryOnly).Select(f => f.FullName));
      foreach (var file in toDelete)
      {
        File.Delete(file);
      }
    }

    /// <summary>
    /// Вернуть название папки.
    /// </summary>
    /// <param name="path">Путь к папке. Абсолютный или относительный.</param>
    /// <returns>Название папки. '.\Folder\' -> 'Folder'</returns>
    private static string GetFolderName(string path)
    {
      return path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                      StringSplitOptions.RemoveEmptyEntries)
                 .Last();
    }

    /// <summary>
    /// Добавить файлы в архив.
    /// </summary>
    /// <param name="archive">Существующий архив.</param>
    /// <param name="folder">Папка, файлы которой необходимо запаковать.</param>
    private static void AddToArchive(string archive, string folder)
    {
      try
      {
        using (var zip = ZipFile.Open(archive, ZipArchiveMode.Update, Encoding.UTF8))
        {
          var directories = new DirectoryInfo(folder);
          var files = directories.GetFiles("*", SearchOption.TopDirectoryOnly).Select(f => f.FullName)
              .Except(directories.GetFiles(ArchivePattern, SearchOption.TopDirectoryOnly).Select(f => f.FullName));
          foreach (var file in files)
          {
            var fileName = file.Replace(directories.FullName + Path.DirectorySeparatorChar, string.Empty).Replace(directories.FullName, string.Empty);
            var fileInZip = zip.Entries.FirstOrDefault(f => f.FullName == fileName);
            if (fileInZip != null)
              fileInZip.Delete();
            zip.CreateEntryFromFile(file, fileName, CompressionLevel.NoCompression);
          }
        }
      }
      catch (InvalidDataException ex)
      {
        BackupFile.MoveToBackup(archive);
        ZipFile.CreateFromDirectory(folder, archive, CompressionLevel.NoCompression, false, Encoding.UTF8);
        var text = string.Format(
                "Не удалось прочитать архив {0} для записи в него папки {1}. \r\n Существующий файл был переименован в {2}. В {3} только содержимое указанной папки.",
                archive, folder, archive + ".bak", archive);
        Log.Exception(ex, text);
      }
    }
  }

  class BackupFile
  {
    private const string BackupFormat = ".dbak";

    internal static void MoveToBackup(string fileName, bool deleteExistBackup = false)
    {
      var backupFileName = fileName + BackupFormat;
      if (File.Exists(backupFileName))
      {
        if (deleteExistBackup)
          File.Delete(backupFileName);
        else
        {
          File.Move(backupFileName, GetNewBackupFileName(backupFileName));
        }
      }
      File.Move(fileName, backupFileName);
    }

    private static string GetNewBackupFileName(string fileName)
    {
      var onlyName = Path.GetFileName(fileName);
      var folder = Path.GetDirectoryName(fileName);
      var backups = Directory.GetFiles(folder, onlyName + "*");
      var backup = backups.Select(Path.GetFileName).OrderBy(s => s.Length).Last();

      var id = new String(backup.Where(Char.IsDigit).ToArray());
      var newId = string.IsNullOrWhiteSpace(id) ? "1" : (int.Parse(id) + 1).ToString(CultureInfo.InvariantCulture);
      var result = string.IsNullOrWhiteSpace(id) ? backup + newId : backup.Replace(id, newId);
      return result;
    }
  }
}
