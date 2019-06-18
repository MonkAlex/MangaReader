namespace MangaReader.Avalonia.Services
{
  public class Dialogs
  {
    /// <summary>
    /// Показать информационное сообщение с кнопкой "Ок".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    public static async System.Threading.Tasks.Task ShowInfo(string title, string text)
    {
      var dialog = new global::Dialogs.Avalonia.Dialog
      {
        Title = title,
        Description = text.PadRight(80)
      };
      var ok = dialog.Buttons.AddButton("Ок");
      dialog.Buttons.DefaultButton = ok;
      dialog.Buttons.CancelButton = ok;

      await dialog.ShowAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Задать вопрос пользователю, варианты "Да\Нет".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    /// <param name="note">Примечание к тексту.</param>
    /// <returns>True, если выбрано Да.</returns>
    public static async System.Threading.Tasks.Task<bool> ShowYesNoDialog(string title, string text, string note)
    {
      var (result, _) = await ShowYesNoDialog(title, text, note, string.Empty);
      return result;
    }


    /// <summary>
    /// Задать вопрос пользователю, варианты "Да\Нет".
    /// </summary>
    /// <param name="title">Заголовок окна.</param>
    /// <param name="text">Текст.</param>
    /// <param name="note">Примечание к тексту.</param>
    /// <param name="checkbox">Текст для галочки.</param>
    /// <returns>True, если выбрано Да.</returns>
    public static async System.Threading.Tasks.Task<(bool dialogResult, bool checkboxValue)> ShowYesNoDialog(string title, string text, string note, string checkbox)
    {
      var dialog = new global::Dialogs.Avalonia.Dialog
      {
        Title = title,
        Description = (text + System.Environment.NewLine + note).PadRight(80)
      };

      global::Dialogs.Controls.BoolControl checkboxControl = null;
      if (!string.IsNullOrWhiteSpace(checkbox))
      {
        checkboxControl = new global::Dialogs.Controls.BoolControl();
        dialog.Controls.Add(checkboxControl);
        checkboxControl.Name = checkbox;
      }

      var yes = dialog.Buttons.AddButton("Да");
      var no = dialog.Buttons.AddButton("Нет");

      var result = false;
      var checkboxValue = false;
      if (await dialog.ShowAsync().ConfigureAwait(true) == yes)
      {
        result = true;
        checkboxValue = checkboxControl?.Value == true;
      }

      return (result, checkboxValue);
    }
  }
}
