using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class ProxySettingSelectorModel : BaseViewModel
  {
    public ProxySettingModel SelectedProxySettingModel
    {
      get => selectedProxySettingModel;
      set
      {
        if (!Equals(selectedProxySettingModel, value))
        {
          if (value != null && value.Id == default)
          {
            lastValidProxySettingModel = selectedProxySettingModel;
            CreateNew().LogException();
          }
          selectedProxySettingModel = value;
          OnPropertyChanged();
        }
      }
    }

    private ProxySettingModel selectedProxySettingModel;
    private ProxySettingModel lastValidProxySettingModel;

    public ObservableCollection<ProxySettingModel> ProxySettingModels { get; private set; }

    public async Task CreateNew()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = new ProxySetting(ProxySettingType.Manual);
        await context.Save(setting).ConfigureAwait(true);

        var model = new ProxySettingModel(setting);
        model.Show();
        if (model.IsSaved)
        {
          this.ProxySettingModels.Insert(ProxySettingModels.Count - 1, model);
          this.SelectedProxySettingModel = model;
        }
        else
        {
          await context.Delete(setting).ConfigureAwait(true);
          SelectedProxySettingModel = lastValidProxySettingModel;
        }
      }
    }

    public void EditModel()
    {
      SelectedProxySettingModel.Show();
    }

    public ProxySettingSelectorModel(ProxySetting proxySetting, ObservableCollection<ProxySettingModel> sharedProxySettingModels)
    {
      this.ProxySettingModels = sharedProxySettingModels;
      this.SelectedProxySettingModel = ProxySettingModels.FirstOrDefault(m => m.Id == proxySetting.Id);
    }
  }
}
