using System.ComponentModel;
using System.Threading.Tasks;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class RemoveSelectedProxyCommand : BaseCommand
  {
    private readonly ProxySettingSelectorModel model;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && model.SelectedProxySettingModel?.IsManual == true;
    }

    public override Task Execute(object parameter)
    {
      var selected = model.SelectedProxySettingModel;
      var models = model.ProxySettingModels;
      var index = models.IndexOf(selected);
      var next = models.Count > index + 1 ? models[index + 1] : models[index - 1];
      models.Remove(selected);
      model.SelectedProxySettingModel = next;
      return Task.CompletedTask;
    }

    public RemoveSelectedProxyCommand(ProxySettingSelectorModel model)
    {
      this.model = model;
      this.model.PropertyChanged += ModelOnPropertyChanged;
    }

    private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ProxySettingSelectorModel.SelectedProxySettingModel))
        this.OnCanExecuteChanged();
    }
  }
}
