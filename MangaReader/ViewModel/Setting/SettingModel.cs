using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class SettingModel : BaseViewModel
  {
    private SettingViewModel selectedModel;
    public AppSettingModel AppSetting { get; private set; }

    public ObservableCollection<SettingViewModel> Views { get; private set; }

    public SettingViewModel SelectedModel
    {
      get { return selectedModel; }
      set
      {
        SyncLoginOnModelChange(selectedModel);
        selectedModel = value;
        OnPropertyChanged();
      }
    }

    public ICommand Save { get; private set; }

    public SettingModel()
    {
      using (var context = Repository.GetEntityContext("Load all settings"))
      {
        var proxySettingModels = new ObservableCollection<ProxySettingModel>(context.Get<ProxySetting>().Select(s => new ProxySettingModel(s)));
        proxySettingModels.Add(ProxySettingModel.CreateEmptyModel());

        this.AppSetting = new AppSettingModel(proxySettingModels);
        this.Views = new ObservableCollection<SettingViewModel>();
        this.Save = new SaveSettingsCommand(this);

        this.Views.Add(this.AppSetting);
        foreach (var setting in context.Get<Core.Services.MangaSetting>())
          this.Views.Add(new MangaSettingModel(setting, proxySettingModels));
        this.SelectedModel = this.AppSetting;
      }
    }

    private void SyncLoginOnModelChange(SettingViewModel model)
    {
      if (model is MangaSettingModel mangaSettings)
      {
        var someLoginView = this.Views
          .OfType<MangaSettingModel>()
          .Where(m => !Equals(m, mangaSettings) && Equals(m.Login.LoginId, mangaSettings.Login.LoginId))
          .ToList();
        foreach (var settingModel in someLoginView)
        {
          settingModel.Login.Login = mangaSettings.Login.Login;
          mangaSettings.Login.Password = mangaSettings.Login.Password;
        }
      }
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
