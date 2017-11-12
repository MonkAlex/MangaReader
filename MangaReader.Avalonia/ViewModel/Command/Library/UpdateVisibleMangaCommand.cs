using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private readonly LibraryContentViewModel viewModel;

    public override async void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
        await Library.ThreadAction(() => Library.Update(viewModel.FilteredItems.Select(i => i.Id), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
    }

    public UpdateVisibleMangaCommand(LibraryContentViewModel viewModel, LibraryViewModel model) : base(model)
    {
      this.viewModel = viewModel;
      this.Name = "Обновить";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}