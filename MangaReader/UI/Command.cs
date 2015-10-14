using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.UI.MainForm;
using Ookii.Dialogs.Wpf;

namespace MangaReader.UI
{
  public static class Command
  {
    public static RoutedUICommand UpdateCurrent = new RoutedUICommand("Обновить", "UpdateCurrent", typeof(Command));

    public static RoutedUICommand UpdateAll = new RoutedUICommand("Обновить всё", "UpdateAll", typeof(Command));

    public static RoutedUICommand ShowSettings = new RoutedUICommand(Strings.Library_Action_Settings, "ShowSettings", typeof(Command));

    public static RoutedUICommand OpenFolder = new RoutedUICommand(Strings.Manga_Action_OpenFolder, "OpenFolder", typeof(Command));

    public static RoutedUICommand DeleteManga = new RoutedUICommand(Strings.Manga_Action_Remove, "DeleteManga", typeof(Command));

    public static RoutedUICommand UpdateManga = new RoutedUICommand(Strings.Manga_Action_Update, "UpdateManga", typeof(Command));

    public static RoutedUICommand MangaProperty = new RoutedUICommand(Strings.Manga_Settings, "MangaProperty", typeof(Command));

    public static RoutedUICommand SelectNextManga = new RoutedUICommand("Следующая", "SelectNextManga", typeof(Command));

    public static RoutedUICommand SelectPrevManga = new RoutedUICommand("Предыдущая", "SelectPrevManga", typeof(Command));

    public static RoutedUICommand CheckUpdates = new RoutedUICommand(Strings.Library_CheckUpdate, "CheckUpdates", typeof(Command));

    public static RoutedUICommand ShowUpdateLog = new RoutedUICommand(Strings.Update_Title, "ShowUpdateLog", typeof(Command));

    public static RoutedUICommand ShowAbout = new RoutedUICommand("О программе", "ShowAbout", typeof(Command));

    public static void AddMainMenuCommands(UIElement element)
    {
      /*
        <MenuItem Header="File">
        <MenuItem Header="Add"/>
        <MenuItem Header="Update current"/>
        <MenuItem Header="Update all"/>
        <MenuItem Header="Exit" Command="Close"/>
      </MenuItem>
      <MenuItem Header="Settings">
        
      </MenuItem>
      <MenuItem Header="Help">
        <MenuItem Header="Read Me"/>
        <MenuItem Header="Check updates"/>
        <MenuItem Header="Update history"/>
        <MenuItem Header="About"/>
       */
      AddCommand(ApplicationCommands.New, DoAdd, CanAdd, element);
      AddCommand(UpdateCurrent, DoUpdateCurrent, CanUpdate, element);
      AddCommand(UpdateAll, DoUpdateAll, CanUpdate, element);
      AddCommand(ApplicationCommands.Close, DoClose, CanClose, element);

      AddCommand(ShowSettings, DoShowSettings, CanShowSettings, element);

      AddCommand(CheckUpdates, DoCheckUpdates, CanCheckUpdates, element);
      AddCommand(ShowUpdateLog, DoShowUpdateLog, CanShowUpdateLog, element);
      AddCommand(ShowAbout, DoShowAbout, CanShowAbout, element);

      AddCommand(SelectNextManga, DoSelectNextManga, CanSelectNextManga, element);
      AddCommand(SelectPrevManga, DoSelectPrevManga, CanSelectPrevManga, element);


    }

    public static void AddMangaCommands(UIElement element)
    {
      /*
       * Open
       * Delete
       * Update
       * Property
       */
      AddCommand(OpenFolder, DoOpenFolder, CanOpenFolder, element);
      AddCommand(DeleteManga, DoDeleteManga, CanDeleteManga, element);
      AddCommand(UpdateManga, DoUpdateManga, CanUpdateManga, element);
      AddCommand(MangaProperty, DoMangaProperty, CanMangaProperty, element);
    }

    private static void AddCommand(ICommand command, ExecutedRoutedEventHandler execute, CanExecuteRoutedEventHandler canExecute, UIElement element)
    {
      // Создание привязки.
      CommandBinding bind = new CommandBinding(command);

      // Присоединение обработчика событий.
      bind.Executed += execute;
      bind.CanExecute += canExecute;

      // Регистрация привязки.
      element.CommandBindings.Add(bind);
    }

    private static void CanAdd(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoAdd(object sender, ExecutedRoutedEventArgs e)
    {
      var window = sender as BaseForm;
      var db = new Input { Owner = window };
      if (db.ShowDialog() != true)
        return;
      try
      {
        if (!string.IsNullOrWhiteSpace(db.Result.Text))
          Library.Add(db.Result.Text);
        foreach (var manga in db.LoginBookmarks.Bookmarks.SelectedItems.OfType<Mangas>())
          Library.Add(manga.Uri);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, "Ошибка добавления манги.");
        MessageBox.Show(ex.Message);
      }
    }

    private static void CanClose(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoClose(object sender, ExecutedRoutedEventArgs e)
    {
      Log.Add("Application will be closed.");
      var window = sender as Window;
      if (window != null)
      {
        window.Close();
      }
      else
      {
        Application.Current.Shutdown(0);
      }
    }

    private static void CanUpdate(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoUpdateCurrent(object sender, ExecutedRoutedEventArgs e)
    {
      var baseForm = sender as BaseForm;
      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(baseForm.View.Cast<Mangas>(), LibraryFilter.SortDescription));
      }
    }

    private static void DoUpdateAll(object sender, ExecutedRoutedEventArgs e)
    {
      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(Library.LibraryMangas, LibraryFilter.SortDescription));
      }
    }

    private static void CanShowSettings(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoShowSettings(object sender, ExecutedRoutedEventArgs e)
    {
      var window = sender as Window;
      new SettingsForm { Owner = window }.ShowDialog();
    }

    private static void CanOpenFolder(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoOpenFolder(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as IDownloadable ?? (e.OriginalSource as FrameworkElement).DataContext as IDownloadable;
      if (manga != null && Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }

    private static void CanDeleteManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoDeleteManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.OriginalSource as FrameworkElement).DataContext as Mangas;
      Library.Remove(manga);
    }

    private static void CanUpdateManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoUpdateManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.OriginalSource as FrameworkElement).DataContext as Mangas;
      if (manga != null && Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(manga));
      }
    }

    private static void CanMangaProperty(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoMangaProperty(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.OriginalSource as FrameworkElement).DataContext as Mangas;
      if (manga != null && Library.IsAvaible)
      {
        new MangaForm { DataContext = manga, Owner = sender as Window }.ShowDialog();
        (sender as BaseForm).View.Refresh();
      }
    }

    private static void CanSelectNextManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoSelectNextManga(object sender, ExecutedRoutedEventArgs e)
    {
      var baseForm = sender as BaseForm;
      var filtered = baseForm.View.Cast<Mangas>().ToList();
      var manga = Library.SelectedManga;
      if (manga != null && !Equals(filtered.LastOrDefault(), manga))
      {
        Library.SelectedManga = filtered.Contains(manga) ? 
          filtered.SkipWhile(m => !Equals(m, manga)).Skip(1).FirstOrDefault() : 
          filtered.FirstOrDefault();
        (e.Source as FrameworkElement).DataContext = Library.SelectedManga;
      }
    }

    private static void CanSelectPrevManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoSelectPrevManga(object sender, ExecutedRoutedEventArgs e)
    {
      var baseForm = sender as BaseForm;
      var filtered = baseForm.View.Cast<Mangas>().ToList();
      var manga = Library.SelectedManga;
      if (manga != null && !Equals(filtered.FirstOrDefault(), manga))
      {
        Library.SelectedManga = filtered.Contains(manga) ?
          filtered.TakeWhile(m => !Equals(m, manga)).LastOrDefault() :
          filtered.FirstOrDefault();
        (e.Source as FrameworkElement).DataContext = Library.SelectedManga;
      }
    }

    private static void CanCheckUpdates(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoCheckUpdates(object sender, ExecutedRoutedEventArgs e)
    {
      var dialog = new TaskDialog();
      dialog.WindowTitle = "Обновление";
      var owner = (sender as Window) ?? (e.Parameter as Window);
      if (owner == null)
        owner = Application.Current.MainWindow ?? Application.Current.Windows.Cast<Window>().FirstOrDefault();
      if (Update.CheckUpdate())
      {
        dialog.MainInstruction = "Запустить процесс обновления?";
        dialog.Content = string.Format("Доступно обновление с версии {0} на {1}", Update.ClientVersion.ToString(3), Update.ServerVersion.ToString(3));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
        if (dialog.ShowDialog(owner).ButtonType == ButtonType.Yes)
          Update.StartUpdate();
      }
      else
      {
        dialog.MainInstruction = "Обновлений не найдено.";
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
        dialog.ShowDialog(owner);
      }
    }

    private static void DoShowUpdateLog(object sender, ExecutedRoutedEventArgs e)
    {
      new VersionHistory((Window)sender).ShowDialog();
    }

    private static void CanShowUpdateLog(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void CanShowAbout(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoShowAbout(object sender, ExecutedRoutedEventArgs e)
    {
      MessageBox.Show("Тут могла быть ваша реклама.");
    }
  }
}
