using System.Linq;
using System.Threading.Tasks;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private readonly LibraryViewModel viewModel;

    public override async Task Execute(object parameter)
    {
      if (Library.IsAvaible)
        await Library.ThreadAction(Library.Update(viewModel.FilteredItems.Select(i => i.Id).ToList())).ConfigureAwait(false);
    }

    public UpdateVisibleMangaCommand(LibraryViewModel viewModel, Core.Services.LibraryViewModel model) : base(model)
    {
      this.viewModel = viewModel;
      this.Name = "Обновить библиотеку";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}