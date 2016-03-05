using System;
using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class BaseMangaCommand : BaseCommand
  {
    protected readonly ListCollectionView View;

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var manga = parameter as Mangas;
      if (manga != null)
      {
        this.Execute(manga);
        View.Refresh();
      }
      else
      {
        Log.Add("Cannot run manga command", parameter);
      }
    }

    public virtual void Execute(Mangas manga)
    {
      
    }

    public BaseMangaCommand(ListCollectionView view)
    {
      this.View = view;
      Library.UpdateCompleted += LibraryChanged;
      Library.UpdateStarted += LibraryChanged;
    }

    private void LibraryChanged(object sender, EventArgs eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}