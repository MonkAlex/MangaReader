using System.Linq;
using System.Windows;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.UI.MainForm;

namespace MangaReader.UI
{
  public static class Command
  {
    public static RoutedUICommand SelectNextManga = new RoutedUICommand("Следующая", "SelectNextManga", typeof(Command));

    public static RoutedUICommand SelectPrevManga = new RoutedUICommand("Предыдущая", "SelectPrevManga", typeof(Command));

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
      AddCommand(ShowAbout, DoShowAbout, CanShowAbout, element);

      AddCommand(SelectNextManga, DoSelectNextManga, CanSelectNextManga, element);
      AddCommand(SelectPrevManga, DoSelectPrevManga, CanSelectPrevManga, element);


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
