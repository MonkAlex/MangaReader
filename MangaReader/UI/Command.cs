using System.Windows;
using System.Windows.Input;

namespace MangaReader.UI
{
  public static class Command
  {
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
