using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;
using MangaReader.Avalonia.Services;
using MangaReader.Avalonia.ViewModel.Explorer;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class ShowPropertiesMangaCommand : MultipleMangasBaseCommand
  {
    private readonly INavigator navigator;
    private readonly IFabric<IManga, MangaModel> fabric;

    public override async Task Execute(IEnumerable<IManga> mangas)
    {
      var manga = mangas.Single();
      var searchTab = this.SelectedModels.SingleOrDefault(m => m.Id == manga.Id);
      if (searchTab != null)
      {
        searchTab.UpdateProperties(manga);
      }
      else
      {
        searchTab = fabric.Create(manga);
      }
      
      await navigator.Open(searchTab).ConfigureAwait(true);
    }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Count() == 1;
    }

    public ShowPropertiesMangaCommand(LibraryViewModel model, INavigator navigator, IFabric<IManga, MangaModel> fabric) 
      : base(model.SelectedMangaModels, model.Library)
    {
      this.navigator = navigator;
      this.fabric = fabric;
      CanExecuteNeedSelection = true;
      this.Name = Strings.Manga_Settings;
      this.Icon = "pack://application:,,,/Icons/Manga/settings.png";
    }
  }
}
