using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MangaReader.Avalonia.ViewModel.Command;
using ReactiveUI;

namespace MangaReader.Avalonia.ViewModel
{
  public class ViewModelBase : ReactiveObject
  {
    public void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      IReactiveObjectExtensions.RaiseAndSetIfChanged(this, ref field, value, propertyName);
    }

    public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      IReactiveObjectExtensions.RaisePropertyChanged(this, propertyName);
    }

    protected System.Action<DelegateCommand> SubscribeToCommand(string propertyName)
    {
      return command =>
      this.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          command.OnCanExecuteChanged();
      };
    }
  }
}
