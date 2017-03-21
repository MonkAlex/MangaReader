using Ookii.Dialogs.Wpf;

namespace MangaReader.Services
{
  public class Dialogs
  {
    /// <summary>
    /// Показать информационное сообщение с кнопкой "Ок".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    public static void ShowInfo(string title, string text)
    {
      var owner = WindowHelper.Owner;
      using (var dialog = new TaskDialog())
      {
        dialog.CenterParent = true;
        dialog.WindowTitle = title;
        dialog.MainInstruction = text;
        dialog.MainIcon = TaskDialogIcon.Information;
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
        dialog.ShowDialog(owner);
      }
    }

    /// <summary>
    /// Задать вопрос пользователю, варианты "Да\Нет".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    /// <param name="note">Примечание к тексту.</param>
    /// <returns>True, если выбрано Да.</returns>
    public static bool ShowYesNoDialog(string title, string text, string note)
    {
      var owner = WindowHelper.Owner;
      var result = false;
      using (var dialog = new TaskDialog())
      {
        dialog.CenterParent = true;
        dialog.WindowTitle = title;
        dialog.MainInstruction = text;
        dialog.Content = note;
        dialog.MainIcon = TaskDialogIcon.Information;
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
        if (dialog.ShowDialog(owner).ButtonType == ButtonType.Yes)
          result = true;
      }
      return result;
    }

    /// <summary>
    /// Показать выбор папки.
    /// </summary>
    /// <param name="oldPath">Текущая выбранная папка.</param>
    /// <returns>Выбранная папка. Если выбор был отменен, вернется oldPath.</returns>
    public static string SelectFolder(string oldPath)
    {
      var dialog = new VistaFolderBrowserDialog();
      dialog.SelectedPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(oldPath));
      if (dialog.ShowDialog() == true)
      {
        return dialog.SelectedPath + System.IO.Path.DirectorySeparatorChar;
      }

      return oldPath;
    }
  }
}