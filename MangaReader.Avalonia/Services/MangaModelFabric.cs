using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

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
    private readonly IFabric<PreviewFoundMangaCommand> previewFoundMangaCommand;

    public MangaSearchViewModel Create(IManga input)
    {
      return new MangaSearchViewModel(input, previewFoundMangaCommand.Create());
    }

    public MangaSearchViewModelFabric(IFabric<PreviewFoundMangaCommand> previewFoundMangaCommand)
    {
      this.previewFoundMangaCommand = previewFoundMangaCommand;
    }
  }

  public class MangaSettingsViewModelFabric : IFabric<MangaSetting, MangaSettingsViewModel>
  {
    private readonly INavigator navigator;

    public MangaSettingsViewModel Create(MangaSetting input)
    {
      return new MangaSettingsViewModel(input, navigator);
    }

    public MangaSettingsViewModelFabric(INavigator navigator)
    {
      this.navigator = navigator;
    }
  }
  
  public class PreviewFoundMangaCommandFabric : IFabric<PreviewFoundMangaCommand>
  {
    private readonly INavigator navigator;
    private readonly ITaskFabric<IManga, MangaModel> mangaModelFabric;

    public PreviewFoundMangaCommand Create()
    {
      return new PreviewFoundMangaCommand(navigator, mangaModelFabric);
    }

    public PreviewFoundMangaCommandFabric(INavigator navigator, ITaskFabric<IManga, MangaModel> mangaModelFabric)
    {
      this.navigator = navigator;
      this.mangaModelFabric = mangaModelFabric;
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

  public interface IFabric<out TOutput>
  {
    TOutput Create();
  }
 
}
