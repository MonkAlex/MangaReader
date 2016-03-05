using System;
using System.Linq;
using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateVisibleMangaCommand : BaseCommand
  {
    private readonly ListCollectionView view;

    public override string Name { get { return "Обновить"; } }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(view.Cast<Mangas>(), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
      }
    }

    public UpdateVisibleMangaCommand(ListCollectionView view)
    {
      this.view = view;
      Library.UpdateCompleted += LibraryChanged;
      Library.UpdateStarted += LibraryChanged;
    }

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    private void LibraryChanged(object sender, EventArgs eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}