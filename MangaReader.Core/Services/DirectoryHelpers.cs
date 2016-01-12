using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MangaReader.Services
{
  public static class DirectoryHelpers
  {
    internal static bool CopyDirectory(string sourceFolder, string destFolder)
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
          if (!CopyDirectory(folder, dest))
            return false;
        }
        return true;
      }
      catch (Exception e)
      {
        Log.Exception(e, string.Format("Не удалось скопировать {0} в {1}.", sourceFolder, destFolder));
        return false;
      }
    }

    internal static bool MoveDirectory(string sourceFolder, string destFolder)
    {
      try
      {
        if (CopyDirectory(sourceFolder, destFolder))
          Directory.Delete(sourceFolder, true);
        else
          throw new Exception(string.Format("Не удалось скопировать {0} в {1}.", sourceFolder, destFolder));
        return true;
      }
      catch (Exception ex)
      {
        Log.Exception(ex, string.Format("Не удалось переместить {0} в {1}.", sourceFolder, destFolder));
        return false;
      }
    }

    /// <summary>
    /// Очистка пути от недопустимых символов.
    /// </summary>
    /// <param name="name">Путь.</param>
    /// <returns>Исправленный путь.</returns>
    public static string MakeValidPath(string name)
    {
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
  }
}