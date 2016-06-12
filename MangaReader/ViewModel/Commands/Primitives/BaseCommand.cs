using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class BaseCommand : ICommand, INotifyPropertyChanged
  {
    private string name;
    private string icon;

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public string Icon
    {
      get { return icon; }
      set
      {
        icon = value;
        OnPropertyChanged();
      }
    }

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
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}