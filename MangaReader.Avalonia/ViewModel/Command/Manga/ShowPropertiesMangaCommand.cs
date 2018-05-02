using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
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
#warning Свойства пока не отображаем.
      // model.Show();
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