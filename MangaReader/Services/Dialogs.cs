using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands;
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
      return ShowYesNoDialog(title, text, note, string.Empty).Item1;
    }


    /// <summary>
    /// Задать вопрос пользователю, варианты "Да\Нет".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    /// <param name="note">Примечание к тексту.</param>
    /// <param name="checkbox">Текст для галочки.</param>
    /// <returns>True, если выбрано Да.</returns>
    public static Tuple<bool, bool> ShowYesNoDialog(string title, string text, string note, string checkbox)
    {
      var owner = WindowHelper.Owner;
      var result = false;
      var checkboxValue = false;
      using (var dialog = new TaskDialog())
      {
        dialog.CenterParent = true;
        dialog.WindowTitle = title;
        dialog.MainInstruction = text;
        dialog.Content = note;
        dialog.MainIcon = TaskDialogIcon.Information;
        if (!string.IsNullOrWhiteSpace(checkbox))
          dialog.VerificationText = checkbox;
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
        if (dialog.ShowDialog(owner).ButtonType == ButtonType.Yes)
        {
          result = true;
          checkboxValue = dialog.IsVerificationChecked;
        }
      }
      return new Tuple<bool, bool>(result, checkboxValue);
    }

    public static void OpenSettingsOnPathNotExists(IEnumerable<MangaSetting> mangaSettings)
    {
      var owner = WindowHelper.Owner;
      using (var dialog = new TaskDialog())
      {
        dialog.CenterParent = true;
        dialog.WindowTitle = "Папки загрузки недоступны";
        dialog.MainInstruction = "Папки загрузки недоступны, рекомендуется исправить их в настройках.";
        dialog.Content = "Недоступны папки:\r\n" + string.Join(Environment.NewLine, mangaSettings.Select(m => $"{m.MangaName}: {DirectoryHelpers.GetAbsoluteFolderPath(m.Folder)}"));
        dialog.MainIcon = TaskDialogIcon.Error;
        var settings = new TaskDialogButton(Strings.Library_Action_Settings);
        dialog.Buttons.Add(settings);
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Cancel));
        Client.Dispatcher.Invoke(() =>
        {
          if (dialog.ShowDialog(owner) == settings)
          {
            new ShowSettingCommand(WindowHelper.Library).Execute(null);
          }
        });
      }
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