using System.ComponentModel;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class LibraryBaseCommand : BaseCommand
  {
    protected LibraryViewModel Library { get; }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Library.IsAvaible;
    }

    public LibraryBaseCommand(LibraryViewModel model)
    {
      Library = model;
      Library.PropertyChanged += LibraryOnPropertyChanged;
    }

    private void LibraryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(Library.IsAvaible) ||
        args.PropertyName == nameof(Library.IsPaused))
        OnCanExecuteChanged();
    }
  }
}