using System;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateMangaCommand : BaseCommand
  {
    private readonly Mangas manga;

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      Library.ThreadAction(() => Library.Update(manga));
    }

    public UpdateMangaCommand(Mangas manga)
    {
      this.manga = manga;
      Library.UpdateCompleted += LibraryChanged;
      Library.UpdateStarted += LibraryChanged;
    }

    private void LibraryChanged(object sender, EventArgs eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}