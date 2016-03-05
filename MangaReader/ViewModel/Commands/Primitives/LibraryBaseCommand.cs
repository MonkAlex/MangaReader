using System;
using MangaReader.Manga;
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
      Library.UpdateCompleted += LibraryChanged;
      Library.UpdateStarted += LibraryChanged;
      Library.UpdateMangaCompleted += LibraryOnUpdateMangaCompleted;
    }

    private void LibraryOnUpdateMangaCompleted(object sender, Mangas mangas)
    {
      OnCanExecuteChanged();
    }

    private void LibraryChanged(object sender, EventArgs eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}