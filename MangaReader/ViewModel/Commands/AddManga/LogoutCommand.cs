using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogoutCommand : BaseCommand
  {
    private MangaSetting setting;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      setting.Login.Logout();
    }

    public LogoutCommand(MangaSetting setting)
    {
      this.setting = setting;
    }
  }
}