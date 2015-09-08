using MangaReader.Manga;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MangaReader.Services
{
  public static class Compression
  {
    public const string ArchiveFormat = ".cbz";
    public const string ArchivePattern = "*.cbz";
    public const string Separator = " ";

    public enum CompressionMode
    {
      Manga,
      Volume,
      Chapter
    }

    /// <summary>
    /// Получить упаковку манги по умолчанию.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <returns>Режим упаковки.</returns>
    public static CompressionMode? GetDefaultCompression(this Mangas manga)
    {
      CompressionMode? mode = null;
      if (Mapping.Environment.Initialized)
      {
        var setting = Settings.MangaSettings.SingleOrDefault(s => Equals(s.Manga, manga.GetType().MangaType()));
        if (setting != null)
          mode = setting.DefaultCompression;
      }

      if (mode == null || !manga.AllowedCompressionModes.Any(m => Equals(m, mode)))
        mode = manga.AllowedCompressionModes.FirstOrDefault();

      return mode;
    }

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
        var chapters = Directory.GetDirectories(volume);
        foreach (var chapter in chapters)
        {
          var acr = string.Concat(volume, Path.DirectorySeparatorChar,
              GetFolderName(chapter), ArchiveFormat);
          files = AddToArchive(acr, chapter);
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
        var acr = string.Concat(message, GetFolderName(volume), ArchiveFormat);
        files = AddToArchive(acr, volume);
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
      DeleteCompressedFiles(files, message);
      Log.Add(string.Format("Compression: End {0}.", message));
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
        var directories = new DirectoryInfo(folder);
        var files = directories
          .GetFiles("*", SearchOption.AllDirectories)
          .Select(f => f.FullName)
          .Except(directories
            .GetFiles(ArchivePattern, SearchOption.AllDirectories)
            .Select(f => f.FullName))
          .ToList();
        if (!files.Any()) 
          return packedFiles;

        using (var zip = ZipFile.Open(archive, archiveMode, Encoding.UTF8))
        {
          foreach (var file in files)
          {
            var fileName = file.Replace(directories.FullName, string.Empty).Trim(Path.DirectorySeparatorChar);
            if (archiveMode == ZipArchiveMode.Update)
            {
              var fileInZip = zip.Entries.SingleOrDefault(f => f.FullName == fileName);
              if (fileInZip != null)
                fileInZip.Delete();
            }
            zip.CreateEntryFromFile(file, fileName, CompressionLevel.NoCompression);
            packedFiles.Add(file);
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
        Log.Exception(ex);

        if (archiveMode == ZipArchiveMode.Update)
          Backup.MoveToBackup(archive);

        return new List<string>();
      }
    }

    private static void DeleteCompressedFiles(List<string> files, string folder)
    {
      foreach (var file in files)
      {
        File.Delete(file);
      }
      DeleteEmptyFolder(folder);
    }

    private static void DeleteEmptyFolder(string folder)
    {
      var subfolders = Directory.GetDirectories(folder).ToList();
      foreach (var subfolder in subfolders)
      {
        DeleteEmptyFolder(subfolder);
      }

      try
      {
        Directory.Delete(folder);
      }
      catch (IOException)
      {
      }
    }
  }
}
