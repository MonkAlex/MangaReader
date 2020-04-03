using System.Globalization;
using System.IO;
using System.Linq;

namespace MangaReader.Core.Services
{
  internal class Backup
  {
    private const string BackupFormat = ".dbak";

    // Стратегия по умолчанию - file -> file.dbak. Если dbak файл уже существует, то file -> file.dbak1..N
    // Если перемещение недоступно, выполняем копирование по тем же стратегиям.
    internal static bool MoveToBackup(string fileName)
    {
      var backupFileName = fileName + BackupFormat;
      backupFileName = GetNewBackupFileName(backupFileName);
      try
      {
        File.Move(fileName, backupFileName);
        return true;
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, "Перемещение файла не удалось, пытаемся скопировать.");
        try
        {
          File.Copy(fileName, backupFileName, true);
          return true;
        }
        catch (System.Exception subException)
        {
          Log.Exception(subException, "Создание копии вместо перемещения тоже не удалось.");
          return false;
        }
      }
    }

    private static string GetNewBackupFileName(string fileName)
    {
      var onlyName = Path.GetFileName(fileName);
      var folder = Path.GetDirectoryName(fileName);
      var backups = Directory.GetFiles(folder, onlyName + "*");
      var backup = backups.Select(Path.GetFileName).OrderBy(s => s.Length).Last();

      var id = new string(Path.GetExtension(backup).Where(char.IsDigit).ToArray());
      var newId = string.IsNullOrWhiteSpace(id) ? "1" : (int.Parse(id) + 1).ToString(CultureInfo.InvariantCulture);
      var result = string.IsNullOrWhiteSpace(id) ? backup + newId : backup.Replace(id, newId);
      return Path.Combine(folder, result);
    }
  }
}
