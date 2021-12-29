using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.Services
{
  public class MangaModelFabric : IFabric<IManga, MangaModel>, ITaskFabric<IManga, MangaModel>
  {
    private readonly INavigator navigator;
    private readonly MangaSaveCommand saveCommand;
    private readonly OpenFolderCommand openFolderCommand;

    async Task<MangaModel> ITaskFabric<IManga, MangaModel>.Create(IManga input)
    {
      var mangaModel = GetMangaModel(input);
        
      var covers = await input.Parser.GetPreviews(input).ConfigureAwait(true);
      mangaModel.Cover = covers.FirstOrDefault();

      return mangaModel;
    }
    
    MangaModel IFabric<IManga, MangaModel>.Create(IManga input)
    {
      return GetMangaModel(input);
    }

    private MangaModel GetMangaModel(IManga input)
    {
      return new MangaModel(input, saveCommand, openFolderCommand, navigator);
    }

    public MangaModelFabric(INavigator navigator, MangaSaveCommand saveCommand, OpenFolderCommand openFolderCommand)
    {
      this.navigator = navigator;
      this.saveCommand = saveCommand;
      this.openFolderCommand = openFolderCommand;
    }
  }

  public class MangaSearchViewModelFabric : IFabric<IManga, MangaSearchViewModel>
  {
    private readonly PreviewFoundMangaCommand previewFoundMangaCommand;

    public MangaSearchViewModel Create(IManga input)
    {
      return new MangaSearchViewModel(input, previewFoundMangaCommand);
    }

    public MangaSearchViewModelFabric(PreviewFoundMangaCommand previewFoundMangaCommand)
    {
      this.previewFoundMangaCommand = previewFoundMangaCommand;
    }
  }

  public interface ITaskFabric<in TInput, TOutput>
  {
    Task<TOutput> Create(TInput input);
  }

  public interface IFabric<in TInput, out TOutput>
  {
    TOutput Create(TInput input);
  }
 
}
