using System.Linq;
using System.Windows.Data;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private readonly ListCollectionView view;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(view.OfType<MangaBaseModel>().Select(m => m.Manga), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
      }
    }

    public UpdateVisibleMangaCommand(ListCollectionView view, LibraryViewModel model) : base(model)
    {
      this.view = view;
      this.Name = "Обновить";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}