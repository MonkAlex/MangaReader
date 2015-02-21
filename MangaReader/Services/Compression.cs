using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader.Services
{
  public class Compression
  {
    public const string ArchiveFormat = ".cbz";
    public const string ArchivePattern = "*.cbz";
    public const string Separator = " ";

    /// <summary>
    /// Упаковка всех глав.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static List<string> CompressChapters(string message)
    {
      var files = new List<string>();

      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.Add(string.Format("Compression: Start {0}.", message));
      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        Log.Add(string.Format("Compression: Start volume {0}.", volume));
        var chapters = Directory.GetDirectories(volume);
        foreach (var chapter in chapters)
        {
          Log.Add(string.Format("Compression: Start chapter {0}.", chapter));
          var acr = string.Concat(volume, Path.DirectorySeparatorChar,
              GetFolderName(message), Separator,
              GetFolderName(volume), Separator,
              GetFolderName(chapter), ArchiveFormat);
          files = AddToArchive(acr, chapter);
          Log.Add(string.Format("Compression: Packed to {0}.", acr));
          DeleteCompressedFiles(files, chapter);
        }
      }
      Log.Add(string.Format("Compression: End {0}.", message));
      return files;
    }

    /// <summary>
    /// Упаковка всех томов.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static List<string> CompressVolumes(string message)
    {
      var files = new List<string>();

      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.Add(string.Format("Compression: Start {0}.", message));
      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        Log.Add(string.Format("Compression: Start volume {0}.", volume));
        var acr = string.Concat(message, GetFolderName(message), Separator, GetFolderName(volume), ArchiveFormat);
        files = AddToArchive(acr, volume);
        Log.Add(string.Format("Compression: Packed to {0}.", acr));
        DeleteCompressedFiles(files, volume);
      }
      Log.Add(string.Format("Compression: End {0}.", message));
      return files;
    }

    /// <summary>
    /// Упаковка всей манги.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public static List<string> CompressManga(string message)
    {
      var files = new List<string>();

      message = Page.MakeValidPath(message) + Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == Settings.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.Add(string.Format("Compression: Start {0}.", message));
      var acr = string.Concat(message, GetFolderName(message), ArchiveFormat);
      files = AddToArchive(acr, message);
      Log.Add(string.Format("Compression: Packed to {0}.", acr));
      DeleteCompressedFiles(files, message);
      return files;
    }

    /// <summary>
    /// Вернуть название папки.
    /// </summary>
    /// <param name="path">Путь к папке. Абсолютный или относительный.</param>
    /// <returns>Название папки. '.\Folder\' -> 'Folder'</returns>
    private static string GetFolderName(string path)
    {
      return path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                      StringSplitOptions.RemoveEmptyEntries)
                 .Last();
    }

    /// <summary>
    /// Добавить файлы в архив.
    /// </summary>
    /// <param name="archive">Существующий архив.</param>
    /// <param name="folder">Папка, файлы которой необходимо запаковать.</param>
    private static List<string> AddToArchive(string archive, string folder)
    {
      var archiveMode = File.Exists(archive) ? ZipArchiveMode.Update : ZipArchiveMode.Create;
      if (archiveMode == ZipArchiveMode.Update)
        Log.Add(string.Format("Compression: archive {0} already exists, add folder {1}.", archive, folder));
      try
      {
        var packedFiles = new List<string>();
        using (var zip = ZipFile.Open(archive, archiveMode, Encoding.UTF8))
        {
          var directories = new DirectoryInfo(folder);
          var files = directories
            .GetFiles("*", SearchOption.AllDirectories)
            .Select(f => f.FullName)
            .Except(directories
              .GetFiles(ArchivePattern, SearchOption.AllDirectories)
              .Select(f => f.FullName))
            .ToList();
          foreach (var file in files)
          {
            var fileName = file.Replace(directories.FullName, string.Empty).Trim(Path.DirectorySeparatorChar);
            if (archiveMode == ZipArchiveMode.Update)
            {
              var fileInZip = zip.Entries.SingleOrDefault(f => f.FullName == fileName);
              if (fileInZip != null)
              {
                Log.Add(string.Format("Compression: delete file {0} from archive {1}.", fileInZip.FullName, archive));
                fileInZip.Delete();
              }
            }
            zip.CreateEntryFromFile(file, fileName, CompressionLevel.NoCompression);
            packedFiles.Add(file);
            Log.Add(string.Format("Compression: File {0} add to archive {1}.", fileName, archive));
          }
        }
        return packedFiles;
      }
      catch (InvalidDataException ex)
      {
        Backup.MoveToBackup(archive);
        ZipFile.CreateFromDirectory(folder, archive, CompressionLevel.NoCompression, false, Encoding.UTF8);
        var text = string.Format(
          "Не удалось прочитать архив {0} для записи в него папки {1}. \r\n Существующий файл был переименован в {2}. В {3} только содержимое указанной папки.",
          archive, folder, archive + ".bak", archive);
        Log.Exception(ex, text);
        return new List<string>();
      }
      catch (Exception ex)
      {
        if (archiveMode == ZipArchiveMode.Update)
          Backup.MoveToBackup(archive);

        Log.Exception(ex);
        return new List<string>();
      }
    }

    private static void DeleteCompressedFiles(List<string> files, string folder)
    {
      foreach (var file in files)
      {
        File.Delete(file);
      }
      var subfolders = Directory.GetDirectories(folder).ToList();
      subfolders.Add(folder);
      foreach (var subfolder in subfolders)
      {
        try
        {
          Directory.Delete(subfolder);
        }
        catch (IOException)
        {
        }
      }
    }
  }
}
