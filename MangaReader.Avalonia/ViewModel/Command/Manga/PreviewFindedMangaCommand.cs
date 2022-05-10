using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.Services;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class PreviewFoundMangaCommand : BaseCommand
  {
    private readonly INavigator navigator;
    private readonly ITaskFabric<IManga, MangaModel> fabric;
    private bool isLoading = false;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !isLoading;
    }

    public override async Task Execute(object parameter)
    {
      if (isLoading)
        return;

      var model = parameter as MangaSearchViewModel;
      if (model == null)
        return;

      try
      {
        this.isLoading = true;
        this.OnCanExecuteChanged();
        this.Name = "Loading...";
        var manga = await Mangas.Create(model.Uri).ConfigureAwait(true);
        if (manga == null)
          return;

        await manga.Refresh().ConfigureAwait(true);

        if (await manga.IsValid().ConfigureAwait(true))
        {
          var mangaModel = await fabric.Create(manga).ConfigureAwait(true);
          await navigator.Open(mangaModel).ConfigureAwait(true);
        }
      }
      finally
      {
        this.isLoading = false;
        this.OnCanExecuteChanged();
        this.Name = "Preview";
      }
    }

    public PreviewFoundMangaCommand(INavigator navigator, ITaskFabric<IManga, MangaModel> fabric)
    {
      this.navigator = navigator;
      this.fabric = fabric;
      this.Name = "Preview";
    }
  }
}
