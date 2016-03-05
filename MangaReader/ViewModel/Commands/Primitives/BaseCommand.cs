using System;
using System.Windows;
using System.Windows.Input;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class BaseCommand : ICommand
  {
    public virtual string Name { get; set; }

    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    public virtual void Execute(object parameter)
    {
      
    }

    public event EventHandler CanExecuteChanged;

    protected virtual void OnCanExecuteChanged()
    {
      if (Application.Current.Dispatcher.CheckAccess())
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      else
        Application.Current.Dispatcher.InvokeAsync(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }
  }
}