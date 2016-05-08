using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class SaveSettingsCommand : BaseCommand
  {
    private readonly SettingModel settingModel;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      foreach (var mangaSetting in settingModel.Views)
        mangaSetting.Save();

      ConfigStorage.Instance.Save();

      var window = ViewService.Instance.TryGet<System.Windows.Window>(settingModel);
      if (window != null)
        window.Close();
    }

    public SaveSettingsCommand(SettingModel model)
    {
      this.settingModel = model;
      this.Name = "Сохранить";
    }
  }
}