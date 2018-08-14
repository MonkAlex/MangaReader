using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ShowPropertiesMangaCommand : MultipleMangasBaseCommand
  {
    private MangaModel model;

    public override void Execute(object parameter)
    {
      model = (MangaModel)parameter;
      base.Execute(parameter);
    }

    public override void Execute(IEnumerable<IManga> mangas)
    {
      model.UpdateProperties(model.ContextManga);
      model.Show();
    }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Count() == 1;
    }

    public ShowPropertiesMangaCommand(MainPageModel model) : base(model)
    {
      CanExecuteNeedSelection = true;
      this.Name = Strings.Manga_Settings;
      this.Icon = "pack://application:,,,/Icons/Manga/settings.png";
    }
  }
}