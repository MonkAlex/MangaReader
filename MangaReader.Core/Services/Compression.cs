﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public class Compression
  {
    public const string ArchiveFormat = ".cbz";
    public const string ArchivePattern = "*.cbz";
    public const string BackupPattern = "*.cbz.dbak*";
    public const string Separator = " ";
    private readonly Environments environments;

    public enum CompressionMode
    {
      Manga,
      Volume,
      Chapter
    }

    public Compression(Environments environments)
    {
      this.environments = environments;
    }

    /// <summary>
    /// Упаковка всех глав.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public List<string> CompressChapters(string message)
    {
      var files = new List<string>();

      message += Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == environments.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.AddFormat("Compression: Start {0}.", message);
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
      Log.AddFormat("Compression: End {0}.", message);
      return files;
    }

    /// <summary>
    /// Упаковка всех томов.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public List<string> CompressVolumes(string message)
    {
      var files = new List<string>();

      message += Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

#warning Проверка не учитывает возможность переопределения папки в настройках типа манги.
      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == environments.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.AddFormat("Compression: Start {0}.", message);
      var volumes = Directory.GetDirectories(message);
      foreach (var volume in volumes)
      {
        var acr = string.Concat(message, GetFolderName(volume), ArchiveFormat);
        files = AddToArchive(acr, volume);
        DeleteCompressedFiles(files, volume);
      }
      Log.AddFormat("Compression: End {0}.", message);
      return files;
    }

    /// <summary>
    /// Упаковка всей манги.
    /// </summary>
    /// <param name="message">Папка манги.</param>
    public List<string> CompressManga(string message)
    {
      var files = new List<string>();

      message += Path.DirectorySeparatorChar;
      if (!Directory.Exists(message))
        return files;

      // Нельзя сжимать папку со всей мангой.
      if (message.Trim(Path.DirectorySeparatorChar) == environments.DownloadFolder.Trim(Path.DirectorySeparatorChar))
        return files;

      Log.AddFormat("Compression: Start {0}.", message);
      var acr = string.Concat(message, GetFolderName(message), ArchiveFormat);
      files = AddToArchive(acr, message);
      DeleteCompressedFiles(files, message);
      Log.AddFormat("Compression: End {0}.", message);
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
        Log.AddFormat("Compression: archive {0} already exists, add folder {1}.", archive, folder);
      var directories = new DirectoryInfo(folder);
      var files = directories
        .GetFiles("*", SearchOption.AllDirectories)
        .Select(f => f.FullName)
        .Except(directories
          .GetFiles(ArchivePattern, SearchOption.AllDirectories)
          .Select(f => f.FullName))
        .Except(directories
          .GetFiles(BackupPattern, SearchOption.AllDirectories)
          .Select(f => f.FullName))
        .ToList();
      try
      {
        if (!files.Any())
          return new List<string>();

        return ZipFiles(archive, archiveMode, files, directories);
      }
      catch (InvalidDataException ex)
      {
        var text = string.Format(
          "Не удалось прочитать архив {0} для записи в него папки {1}. \r\n Существующий файл был переименован в {2}. В {3} только содержимое указанной папки.",
          archive, folder, archive + ".bak", archive);
        Log.Exception(ex, text);
        if (Backup.MoveToBackup(archive))
        {
          return ZipFiles(archive, ZipArchiveMode.Create, files, directories);
        }

        return new List<string>();
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);

        if (archiveMode == ZipArchiveMode.Update)
          Backup.MoveToBackup(archive);

        return new List<string>();
      }
    }

    private static List<string> ZipFiles(string archive, ZipArchiveMode archiveMode, List<string> files, DirectoryInfo directories)
    {
      var packedFiles = new List<string>();
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

    private static void DeleteCompressedFiles(List<string> files, string folder)
    {
      foreach (var file in files)
      {
        File.Delete(file);
      }
      DirectoryHelpers.DeleteEmptyFolder(folder);
    }

  }
}
