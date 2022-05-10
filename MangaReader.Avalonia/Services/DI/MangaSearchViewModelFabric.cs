using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.Services
{
  /// <summary>
  /// Маппинг <see cref="IManga"/> -> <see cref="MangaSearchViewModel"/>
  /// </summary>
  public class MangaSearchViewModelFabric : IFabric<IManga, MangaSearchViewModel>
  {
    private readonly ITaskFabric<IManga, MangaModel> mangaModelFabric;
    private readonly INavigator navigator;

    public MangaSearchViewModel Create(IManga input)
    {
      var previewCommand = new PreviewFoundMangaCommand(navigator, mangaModelFabric);
      return new MangaSearchViewModel(input, previewCommand);
    }

    public MangaSearchViewModelFabric(INavigator navigator, ITaskFabric<IManga, MangaModel> mangaModelFabric)
    {
      this.navigator = navigator;
      this.mangaModelFabric = mangaModelFabric;
    }
  }
 
}
