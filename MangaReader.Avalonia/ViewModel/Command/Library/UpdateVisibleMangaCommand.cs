using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private LibraryViewModel viewModel;

    public override async Task Execute(object parameter)
    {
      if (viewModel == null)
        throw new ArgumentNullException(nameof(viewModel), $"Use method {nameof(SetViewModel)} before run command.");

      if (Library.IsAvaible)
        await Library.ThreadAction(Library.Update(viewModel.FilteredItems.Select(i => i.Id).ToList())).ConfigureAwait(true);
    }

    /// <summary> Dependency >_< </summary>
    internal void SetViewModel(LibraryViewModel viewModel)
    {
      this.viewModel = viewModel;
    }

    public UpdateVisibleMangaCommand(Core.Services.LibraryViewModel model) : base(model)
    {
      this.Name = "Обновить библиотеку";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}
