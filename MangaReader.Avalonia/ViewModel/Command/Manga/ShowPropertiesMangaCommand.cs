using System.Collections.Generic;
using System.Linq;
using Dialogs.Avalonia;
using Dialogs.Buttons;
using MangaReader.Core.Manga;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class ShowPropertiesMangaCommand : MultipleMangasBaseCommand
  {
    private MangaModel model;

    public override async void Execute(IEnumerable<IManga> mangas)
    {
      var dialog = new Dialog();
      dialog.Title = this.Name;
      dialog.Description = "Не реализовано, возможно в след. версии.";
      dialog.Buttons.AddButton(DefaultButtons.OkButton);
      await dialog.ShowAsync();
#warning Свойства пока не отображаем.
      //SelectedModels.SingleOrDefault()?.Show();
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