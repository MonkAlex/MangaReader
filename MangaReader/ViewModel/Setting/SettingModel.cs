using System.Collections.ObjectModel;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class SettingModel : BaseViewModel
  {
    public AppSettingModel AppSetting { get; private set; }

    public ObservableCollection<SettingViewModel> Views { get; private set; }

    public ICommand Save { get; private set; }

    public SettingModel()
    {
      this.AppSetting = new AppSettingModel();
      this.Views = new ObservableCollection<SettingViewModel>();
      this.Save = new SaveSettingsCommand(this);

      this.Views.Add(this.AppSetting);
      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
        this.Views.Add(new MangaSettingModel(setting));
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.ShowDialog();
      }
    }
  }
}