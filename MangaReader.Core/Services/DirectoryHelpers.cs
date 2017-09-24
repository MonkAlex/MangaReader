using System;
using System.IO;
using System.Text.RegularExpressions;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public static class DirectoryHelpers
  {
    public static bool Equals(string one, string two)
    {
      return string.Compare(new DirectoryInfo(one).FullName.TrimEnd('\\'),
        new DirectoryInfo(two).FullName.TrimEnd('\\'),
        StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    private static void CopyDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        if (!Directory.Exists(destFolder))
          Directory.CreateDirectory(destFolder);
        var files = Directory.GetFiles(sourceFolder);
        foreach (var file in files)
        {
          var name = Path.GetFileName(file);
          var dest = Path.Combine(destFolder, name);
          File.Copy(file, dest);
        }
        var folders = Directory.GetDirectories(sourceFolder);
        foreach (var folder in folders)
        {
          var name = Path.GetFileName(folder);
          var dest = Path.Combine(destFolder, name);
          CopyDirectory(folder, dest);
        }
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
        CopyDirectory(sourceFolder, destFolder);
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

    /// <summary>
    /// Очистка пути от недопустимых символов.
    /// </summary>
    /// <param name="name">Путь.</param>
    /// <returns>Исправленный путь.</returns>
    public static string MakeValidPath(string name)
    {
      if (Environment.OSVersion.Platform == PlatformID.Unix ||
          Environment.OSVersion.Platform == PlatformID.MacOSX)
        return name;
      
      const string replacement = ".";
      var matchesCount = Regex.Matches(name, @":\\").Count;
      string correctName;
      if (matchesCount > 0)
      {
        var regex = new Regex(@":", RegexOptions.RightToLeft);
        correctName = regex.Replace(name, replacement, regex.Matches(name).Count - matchesCount);
      }
      else
        correctName = name.Replace(":", replacement);

      var invalidChars = Regex.Escape(string.Concat(new string(Path.GetInvalidPathChars()), "?", "/", "*"));
      var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

      return Regex.Replace(correctName, invalidRegStr, replacement);
    }


    public static string GetAbsoulteFolderPath(this IDownloadable downloadable)
    {
      return GetAbsoulteFolderPath(downloadable.Folder);
    }

    public static string GetAbsoulteFolderPath(string folder)
    {
      if (!Uri.TryCreate(folder, UriKind.RelativeOrAbsolute, out Uri folderUri))
      {
        return folder;
      }
      else if (!folderUri.IsAbsoluteUri)
      {
        return Path.Combine(ConfigStorage.WorkFolder, folder);
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