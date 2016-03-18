using System;
using System.Linq;
using System.Windows.Data;
using MangaReader.Core.Services.Config;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
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
    }
  }
}