using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class LibraryBaseCommand : BaseCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Library.IsAvaible;
    }

    public LibraryBaseCommand()
    {
      Library.AvaibleChanged += LibraryChanged;
    }

    private void LibraryChanged(object sender, bool eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}