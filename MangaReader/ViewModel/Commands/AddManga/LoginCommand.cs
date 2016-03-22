using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LoginCommand : BaseCommand
  {
    private MangaSetting setting;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      setting.Login.DoLogin();
    }

    public LoginCommand(MangaSetting setting)
    {
      this.setting = setting;
    }
  }
}