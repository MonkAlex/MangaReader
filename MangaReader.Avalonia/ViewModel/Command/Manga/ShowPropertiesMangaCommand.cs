using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dialogs.Avalonia;
using Dialogs.Buttons;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;
using MangaReader.Avalonia.ViewModel.Explorer;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class ShowPropertiesMangaCommand : MultipleMangasBaseCommand
  {
    public override Task Execute(IEnumerable<IManga> mangas)
    {
      var manga = mangas.Single();
      var explorer = ExplorerViewModel.Instance;
      var searchTab = explorer.Tabs.OfType<MangaModel>().SingleOrDefault(t => t.Id == manga.Id);
      if (searchTab == null)
      {
        searchTab = this.LibraryModel.SelectedMangaModels.SingleOrDefault(m => m.Id == manga.Id);
        if (searchTab != null)
          searchTab.UpdateProperties(manga);
        else
          searchTab = new MangaModel(manga);
        explorer.Tabs.Add(searchTab);
      }
      else
        searchTab.UpdateProperties(manga);

      explorer.SelectedTab = searchTab;
      
      return Task.CompletedTask;
    }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Count() == 1;
    }

    public ShowPropertiesMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      CanExecuteNeedSelection = true;
      this.Name = Strings.Manga_Settings;
      this.Icon = "pack://application:,,,/Icons/Manga/settings.png";
    }
  }
}