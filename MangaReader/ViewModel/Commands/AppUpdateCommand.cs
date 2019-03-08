using MangaReader.Core.Services;
using MangaReader.Core.Update;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AppUpdateCommand : LibraryBaseCommand
  {
    public override async void Execute(object parameter)
    {
      base.Execute(parameter);
      await Updater.StartUpdate().ConfigureAwait(false);
    }

    public AppUpdateCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Library_CheckUpdate;
      this.Icon = "pack://application:,,,/Icons/Main/update_app.png";
    }
  }
}