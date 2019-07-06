using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public abstract class BaseCommand : ICommand, INotifyPropertyChanged
  {
    private string name;
    private string icon;
    private bool isVisible = true;

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

    public bool IsVisible
    {
      get { return isVisible; }
      set
      {
        isVisible = value;
        OnPropertyChanged();
      }
    }

    bool ICommand.CanExecute(object parameter)
    {
      var canExecute = this.CanExecute(parameter);
      IsVisible = canExecute;
      return canExecute;
    }

    async void ICommand.Execute(object parameter)
    {
      var commandName = Name ?? GetType().Name;
      Log.Add($"Command '{commandName}' started.");
      await Execute(parameter).ConfigureAwait(true);
    }

    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    public abstract Task Execute(object parameter);

    public event EventHandler CanExecuteChanged;

    public virtual void OnCanExecuteChanged()
    {
      void InvokeCanExecuteChanged()
      {
        //HACK: if button deattach from logical tree when isvisible = false, button not attach on next show.
        // Not attach -> not subscribe to CanExecuteChanged.
        if (CanExecuteChanged == null && !IsVisible)
          IsVisible = true;

        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      }

      if (Dispatcher.UIThread.CheckAccess())
        InvokeCanExecuteChanged();
      else
        Dispatcher.UIThread.InvokeAsync(InvokeCanExecuteChanged);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
