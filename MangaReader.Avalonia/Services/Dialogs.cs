namespace MangaReader.Avalonia.Services
{
  public static class Dialogs
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
      var (result, _) = await ShowYesNoDialog(title, text, note, string.Empty).ConfigureAwait(true);
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

    // BUG: https://github.com/AvaloniaUI/Avalonia/issues/2975
    private static readonly bool IsWin32NT = System.Environment.OSVersion.Platform == System.PlatformID.Win32NT;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(System.IntPtr hWnd);

    public static void ActivateWorkaround(this global::Avalonia.Controls.Window window)
    {
      if (ReferenceEquals(window, null))
        throw new System.ArgumentNullException(nameof(window));

      // Call default Activate() anyway.
      window.Activate();

      // Skip workaround for non-windows platforms.
      if (!IsWin32NT)
        return;

      var platformImpl = window.PlatformImpl;
      if (ReferenceEquals(platformImpl, null))
        return;

      var platformHandle = platformImpl.Handle;
      if (ReferenceEquals(platformHandle, null))
        return;

      var handle = platformHandle.Handle;
      if (System.IntPtr.Zero == handle)
        return;

      try
      {
        SetForegroundWindow(handle);
      }
      catch
      {
        // ignored
      }
    }
  }
}
