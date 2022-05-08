using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Account;
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

  public class MangaSettingsViewModelFabric : IFabric<MangaSetting, MangaSettingsViewModel>
  {
    private readonly INavigator navigator;
    private readonly IFabric<ProxySetting, ProxySettingModel> fabric;

    public MangaSettingsViewModel Create(MangaSetting input)
    {
      return new MangaSettingsViewModel(input, navigator, fabric);
    }

    public MangaSettingsViewModelFabric(INavigator navigator, IFabric<ProxySetting, ProxySettingModel> fabric)
    {
      this.navigator = navigator;
      this.fabric = fabric;
    }
  }

  public class ProxySettingModelFabric : IFabric<ProxySetting, ProxySettingModel>
  {
    public ProxySettingModel Create(ProxySetting input)
    {
      return new ProxySettingModel(input);
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
