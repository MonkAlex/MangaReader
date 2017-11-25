using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private readonly LibraryContentViewModel viewModel;

    public override async void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
        await Library.ThreadAction(() => Library.Update(viewModel.FilteredItems.Select(i => i.Id)));
    }

    public UpdateVisibleMangaCommand(LibraryContentViewModel viewModel, LibraryViewModel model) : base(model)
    {
      this.viewModel = viewModel;
      this.Name = "Обновить";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}