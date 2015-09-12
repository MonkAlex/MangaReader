using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.UI.MainForm;
using ThreadState = System.Threading.ThreadState;

namespace MangaReader.UI
{
  public static class Command
  {
    public static RoutedUICommand UpdateCurrent = new RoutedUICommand("UpdateCurrent", "UpdateCurrent", typeof(Command));

    public static RoutedUICommand UpdateAll = new RoutedUICommand("UpdateAll", "UpdateAll", typeof(Command));

    public static RoutedUICommand ShowSettings = new RoutedUICommand("ShowSettings", "ShowSettings", typeof(Command));

    public static RoutedUICommand OpenFolder = new RoutedUICommand("OpenFolder", "OpenFolder", typeof(Command));

    public static RoutedUICommand DeleteManga = new RoutedUICommand("DeleteManga", "DeleteManga", typeof(Command));

    public static RoutedUICommand UpdateManga = new RoutedUICommand("UpdateManga", "UpdateManga", typeof(Command));

    public static RoutedUICommand MangaProperty = new RoutedUICommand("MangaProperty", "MangaProperty", typeof(Command));

    public static RoutedUICommand SelectNextManga = new RoutedUICommand("SelectNextManga", "SelectNextManga", typeof(Command));

    public static RoutedUICommand SelectPrevManga = new RoutedUICommand("SelectPrevManga", "SelectPrevManga", typeof(Command));

    public static RoutedUICommand CheckUpdates = new RoutedUICommand("CheckUpdates", "CheckUpdates", typeof(Command));

    public static RoutedUICommand ShowUpdateLog = new RoutedUICommand("ShowUpdateLog", "ShowUpdateLog", typeof(Command));

    public static RoutedUICommand ShowAbout = new RoutedUICommand("ShowAbout", "ShowAbout", typeof(Command));

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

      if (!string.IsNullOrWhiteSpace(db.Result.Text))
        Library.Add(db.Result.Text);
      foreach (var manga in db.Bookmarks.SelectedItems.OfType<Mangas>())
      {
        Library.Add(manga.Uri);
      }
      //Library.FilterChanged(window);
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

    private static Thread _loadThread;

    private static void CanUpdate(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped) && Library.IsAvaible;
    }

    private static void DoUpdateCurrent(object sender, ExecutedRoutedEventArgs e)
    {
      if (Library.IsAvaible)
      {
        if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
          _loadThread = new Thread(() => Library.Update(Library.FilteredMangas, LibraryFilter.SortDescription));
        if (_loadThread.ThreadState == ThreadState.Unstarted)
          _loadThread.Start();
      }
    }

    private static void DoUpdateAll(object sender, ExecutedRoutedEventArgs e)
    {
      if (Library.IsAvaible)
      {
        if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
          _loadThread = new Thread(() => Library.Update(Library.LibraryMangas, LibraryFilter.SortDescription));
        if (_loadThread.ThreadState == ThreadState.Unstarted)
          _loadThread.Start();
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
      var manga = e.Parameter as IDownloadable ?? (e.Source as FrameworkElement).DataContext as IDownloadable;
      if (manga != null && Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }

    private static void CanDeleteManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoDeleteManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.Source as FrameworkElement).DataContext as Mangas;
      if (manga != null && manga.IsValid())
        manga.Delete();
    }

    private static void CanUpdateManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoUpdateManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.Source as FrameworkElement).DataContext as Mangas;
      if (manga != null && Library.IsAvaible)
      {
        if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
          _loadThread = new Thread(() => Library.Update(manga));
        if (_loadThread.ThreadState == ThreadState.Unstarted)
          _loadThread.Start();
      }
    }

    private static void CanMangaProperty(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoMangaProperty(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = e.Parameter as Mangas ?? (e.Source as FrameworkElement).DataContext as Mangas;
      if (manga != null && Library.IsAvaible)
      {
        new MangaForm { DataContext = manga, Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : null }.ShowDialog();
        //Library.FilterChanged(this);
      }
    }

    private static void CanSelectNextManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoSelectNextManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = Library.SelectedManga;
      if (manga != null && !Equals(Library.FilteredMangas.LastOrDefault(), manga))
      {
        Library.SelectedManga = Library.FilteredMangas.SkipWhile(m => !Equals(m, manga)).Skip(1).FirstOrDefault();
        (e.Source as FrameworkElement).DataContext = Library.SelectedManga;
      }
    }

    private static void CanSelectPrevManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoSelectPrevManga(object sender, ExecutedRoutedEventArgs e)
    {
      var manga = Library.SelectedManga;
      if (manga != null && !Equals(Library.FilteredMangas.FirstOrDefault(), manga))
      {
        Library.SelectedManga = Library.FilteredMangas.TakeWhile(m => !Equals(m, manga)).LastOrDefault();
        (e.Source as FrameworkElement).DataContext = Library.SelectedManga;
      }
    }

    private static void CanCheckUpdates(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoCheckUpdates(object sender, ExecutedRoutedEventArgs e)
    {
      Update.StartUpdate();
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
