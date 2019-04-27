using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public static class DirectoryHelpers
  {
    private static readonly string NormalizationPattern = string.Format(@"([{0}]*\.+$)|([{0}]+)", Regex.Escape(string.Concat(new string(Path.GetInvalidPathChars()), "?", "/", "*", "\"", ":", "\\")));
    private static readonly string[] DosReservedNames = { "CON", "PRN", "AUX", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

    public static bool Equals(string one, string two)
    {
      return string.Compare(new DirectoryInfo(one).FullName.TrimEnd('\\'),
        new DirectoryInfo(two).FullName.TrimEnd('\\'),
        StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    /// <summary>
    /// Проверить вхождение одной папки в другую.
    /// </summary>
    /// <param name="folder">Папка.</param>
    /// <param name="subfolder">Папка, которую проверяем на вхождение.</param>
    /// <returns>True, если subfolder вложена в folder.</returns>
    public static bool IsSubfolder(string folder, string subfolder)
    {
      var folderPaths = folder.Split(Path.DirectorySeparatorChar);
      var subfolderPaths = subfolder.Split(Path.DirectorySeparatorChar);
      return subfolderPaths.Length > folderPaths.Length &&
             subfolderPaths.Take(folderPaths.Length).SequenceEqual(folderPaths);
    }

    private static IEnumerable<string> CopyDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        var result = new List<string>();
        if (!Directory.Exists(destFolder))
          Directory.CreateDirectory(destFolder);
        var files = Directory.GetFiles(sourceFolder);
        foreach (var file in files)
        {
          var name = Path.GetFileName(file);
          var dest = Path.Combine(destFolder, name);
          File.Copy(file, dest);
          result.Add(file);
        }

        var folders = Directory.GetDirectories(sourceFolder);
        foreach (var folder in folders)
        {
          if (DirectoryHelpers.Equals(folder, destFolder))
            continue;

          var name = Path.GetFileName(folder);
          var dest = Path.Combine(destFolder, name);
          result.AddRange(CopyDirectory(folder, dest));
        }

        return result;
      }
      catch (System.Exception e)
      {
        throw new CopyDirectoryFailed("Не удалось скопировать файл или папку. См InnerException.", sourceFolder, destFolder, e);
      }
    }

    internal static void MoveDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        var copied = CopyDirectory(sourceFolder, destFolder);
        if (IsSubfolder(sourceFolder, destFolder))
        {
          foreach (var file in copied)
          {
            File.Delete(file);
          }

          DeleteEmptyFolder(sourceFolder);
        }
        else
          Directory.Delete(sourceFolder, true);
      }
      catch (System.Exception ex)
      {
        throw new CopyDirectoryFailed("Не удалось переместить папку. См InnerException.", sourceFolder, destFolder, ex);
      }
    }

    public static void DeleteDirectory(string folder)
    {
      try
      {
        Directory.Delete(folder, true);
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, $"Не удалось удалить папку {folder}");
      }
    }

    internal static void DeleteEmptyFolder(string folder)
    {
      var subfolders = Directory.GetDirectories(folder).ToList();
      foreach (var subfolder in subfolders)
      {
        DeleteEmptyFolder(subfolder);
      }

      if (!Directory.EnumerateFileSystemEntries(folder).Any())
      {
        try
        {
          Directory.Delete(folder);
        }
        catch (IOException ex)
        {
          Log.Exception(ex);
        }
      }
    }

    /// <summary>
    /// Проверить путь к папке хранения указываемый в настройках.
    /// </summary>
    /// <param name="path">Путь к папке.</param>
    /// <returns>True, если путь в порядке.</returns>
    /// <remarks>Путь должен существовать или быть значением по умолчанию. Не должен заканчиваться относительными путями.</remarks>
    public static bool ValidateSettingPath(string path)
    {
      if (Equals(path, AppConfig.DownloadFolderName) || Equals(path, AppConfig.DownloadFolder))
        return true;

      if (path.TrimEnd(Path.PathSeparator, Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, Path.VolumeSeparatorChar).EndsWith("."))
        return false;

      return Directory.Exists(GetAbsoluteFolderPath(path));
    }


    /// <summary>
    /// Очистка имени от недопустимых символов.
    /// </summary>
    /// <param name="name">Имя.</param>
    /// <returns>Имя без недопустимых символов.</returns>
    /// <remarks>В имени не должно быть разделителей, они будут восприниматься как часть имени.</remarks>
    public static string RemoveInvalidCharsFromName(string name)
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix ||
          Environment.OSVersion.Platform == PlatformID.MacOSX)
        return name;

      const string replacement = ".";

      var folder = Regex.Replace(name, NormalizationPattern, replacement);
      foreach (var reservedName in DosReservedNames)
      {
        var reservedNameWithDot = reservedName + '.';
        if (string.Equals(folder, reservedName, StringComparison.InvariantCultureIgnoreCase))
          folder = replacement + reservedName;
        else if (folder.StartsWith(reservedNameWithDot, StringComparison.InvariantCultureIgnoreCase))
          folder = replacement + reservedNameWithDot + folder.Remove(0, reservedNameWithDot.Length);
      }

      // Если имя оказалось целиком из точек и\или пробелов - заменяем на константу.
      folder = folder.TrimEnd(' ', '.');
      if (string.IsNullOrWhiteSpace(folder))
        folder = "invalid name";

      return folder;
    }


    public static string GetAbsoluteFolderPath(this IDownloadable downloadable)
    {
      return GetAbsoluteFolderPath(downloadable.Folder);
    }

    public static string GetAbsoluteFolderPath(string folder)
    {
      if (!Uri.TryCreate(folder, UriKind.RelativeOrAbsolute, out Uri folderUri))
      {
        return folder;
      }
      else if (!folderUri.IsAbsoluteUri)
      {
        return Path.GetFullPath(Path.Combine(ConfigStorage.WorkFolder, folder));
      }
      else
      {
        return folder;
      }
    }

    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>http://stackoverflow.com/a/32113484/3768545</remarks>
    public static string GetRelativePath(string fromPath, string toPath)
    {
      if (string.IsNullOrEmpty(fromPath))
      {
        throw new ArgumentNullException("fromPath");
      }

      if (string.IsNullOrEmpty(toPath))
      {
        throw new ArgumentNullException("toPath");
      }

      Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
      Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

      if (fromUri.Scheme != toUri.Scheme)
      {
        return toPath;
      }

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
      {
        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      }

      return relativePath;
    }

    private static string AppendDirectorySeparatorChar(string path)
    {
      // Append a slash only if the path is a directory and does not have a slash.
      if (!Path.HasExtension(path) &&
          !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
      {
        return path + Path.DirectorySeparatorChar;
      }

      return path;
    }
  }
}
