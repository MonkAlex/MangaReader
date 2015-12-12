using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MangaReader.Services
{
  internal class Backup
  {
    private const string BackupFormat = ".dbak";

    internal static void MoveToBackup(string fileName, bool deleteExistBackup = false)
    {
      var backupFileName = fileName + BackupFormat;
      try
      {
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
      catch (Exception ex)
      {
        Log.Exception(ex, "Перемещение файла не удалось, пытаемся скопировать.");
        try
        {
          if (File.Exists(backupFileName))
            File.Copy(backupFileName, GetNewBackupFileName(backupFileName));
          File.Copy(fileName, backupFileName, true);
        }
        catch (Exception subException)
        {
          Log.Exception(subException, "Создание копии вместо перемещения тоже не удалось.");
        }
      }
    }

    private static string GetNewBackupFileName(string fileName)
    {
      var onlyName = Path.GetFileName(fileName);
      var folder = Path.GetDirectoryName(fileName);
      var backups = Directory.GetFiles(folder, onlyName + "*");
      var backup = backups.Select(Path.GetFileName).OrderBy(s => s.Length).Last();

      var id = new string(backup.Where(char.IsDigit).ToArray());
      var newId = string.IsNullOrWhiteSpace(id) ? "1" : (int.Parse(id) + 1).ToString(CultureInfo.InvariantCulture);
      var result = string.IsNullOrWhiteSpace(id) ? backup + newId : backup.Replace(id, newId);
      return result;
    }
  }
}