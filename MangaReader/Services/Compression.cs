using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader.Services
{
  class Compression
  {
    /// <summary>
    /// Упаковка всех глав.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static void CompressChapters(string message)
    {
      message = Page.MakeValidPath(message) + "\\";
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim('\\') == Settings.DownloadFolder.Trim('\\'))
        return;

      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        var chapters = Directory.GetDirectories(volume);
        foreach (var chapter in chapters)
        {
          var acr = string.Concat(volume, "\\",
              GetFolderName(message), "_",
              GetFolderName(volume), "_",
              GetFolderName(chapter), ".cbz");
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
      message = Page.MakeValidPath(message) + "\\";
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim('\\') == Settings.DownloadFolder.Trim('\\'))
        return;

      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        var acr = string.Concat(message, GetFolderName(message), "_", GetFolderName(volume), ".cbz");
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
      message = Page.MakeValidPath(message) + "\\";
      if (!Directory.Exists(message))
        return;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim('\\') == Settings.DownloadFolder.Trim('\\'))
        return;

      var acr = string.Concat(message, GetFolderName(message), ".cbz");
      var directories = new DirectoryInfo(message);
      var cbzs = directories.GetFiles("*.cbz", SearchOption.TopDirectoryOnly);
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
                  .Except(directories.GetFiles("*.cbz", SearchOption.TopDirectoryOnly).Select(f => f.FullName));
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
              .Except(directories.GetFiles("*.cbz", SearchOption.TopDirectoryOnly).Select(f => f.FullName));
          foreach (var file in files)
          {
            var fileName = file.Replace(directories.FullName + "\\", string.Empty).Replace(directories.FullName, string.Empty);
            var fileInZip = zip.Entries.FirstOrDefault(f => f.FullName == fileName);
            if (fileInZip != null)
              fileInZip.Delete();
            zip.CreateEntryFromFile(file, fileName, CompressionLevel.NoCompression);
          }
        }
      }
      catch (InvalidDataException ex)
      {
        File.Move(archive, archive + ".bak");
        ZipFile.CreateFromDirectory(folder, archive, CompressionLevel.NoCompression, false, Encoding.UTF8);
        var text = string.Format(
                "Не удалось прочитать архив {0} для записи в него папки {1}. \r\n Существующий файл был переименован в {2}. В {3} только содержимое указанной папки.",
                archive, folder, archive + ".bak", archive);
        Log.Exception(ex, text);
      }
    }
  }
}
