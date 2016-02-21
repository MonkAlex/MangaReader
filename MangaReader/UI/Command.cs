using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.UI.MainForm;
using MangaReader.Update;
using MangaReader.ViewModel;
using Ookii.Dialogs.Wpf;

namespace MangaReader.UI
{
  public static class Command
  {
    public static RoutedUICommand UpdateCurrent = new RoutedUICommand("Обновить", "UpdateCurrent", typeof(Command));

    public static RoutedUICommand UpdateAll = new RoutedUICommand("Обновить всё", "UpdateAll", typeof(Command));

    public static RoutedUICommand DeleteManga = new RoutedUICommand(Strings.Manga_Action_Remove, "DeleteManga", typeof(Command));

    public static RoutedUICommand UpdateManga = new RoutedUICommand(Strings.Manga_Action_Update, "UpdateManga", typeof(Command));

    public static RoutedUICommand MangaProperty = new RoutedUICommand(Strings.Manga_Settings, "MangaProperty", typeof(Command));

    public static RoutedUICommand SelectNextManga = new RoutedUICommand("Следующая", "SelectNextManga", typeof(Command));

    public static RoutedUICommand SelectPrevManga = new RoutedUICommand("Предыдущая", "SelectPrevManga", typeof(Command));

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
      AddCommand(UpdateCurrent, DoUpdateCurrent, CanUpdate, element);
      AddCommand(UpdateAll, DoUpdateAll, CanUpdate, element);

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
    
    private static void CanUpdate(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = Library.IsAvaible;
    }

    private static void DoUpdateCurrent(object sender, ExecutedRoutedEventArgs e)
    {
      var baseForm = sender as BaseForm;
      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(baseForm.Model.View.Cast<Mangas>(), baseForm.Model.LibraryFilter.SortDescription));
      }
    }

    private static void DoUpdateAll(object sender, ExecutedRoutedEventArgs e)
    {
      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(Library.LibraryMangas, ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
      }
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
        new MangaForm { DataContext = manga, Owner = WindowHelper.Owner }.ShowDialog();
        (sender as BaseForm).Model.View.Refresh();
      }
    }

    private static void CanSelectNextManga(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private static void DoSelectNextManga(object sender, ExecutedRoutedEventArgs e)
    {
      var baseForm = sender as BaseForm;
      var filtered = baseForm.Model.View.Cast<Mangas>().ToList();
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
      var filtered = baseForm.Model.View.Cast<Mangas>().ToList();
      var manga = Library.SelectedManga;
      if (manga != null && !Equals(filtered.FirstOrDefault(), manga))
      {
        Library.SelectedManga = filtered.Contains(manga) ?
          filtered.TakeWhile(m => !Equals(m, manga)).LastOrDefault() :
          filtered.FirstOrDefault();
        (e.Source as FrameworkElement).DataContext = Library.SelectedManga;
      }
    }

    private static void DoShowUpdateLog(object sender, ExecutedRoutedEventArgs e)
    {
      // TODO: sender уже не окно.
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
