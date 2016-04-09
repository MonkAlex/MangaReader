using System.Collections.ObjectModel;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class AddNewModel : BaseViewModel
  {
    private string inputText;

    public ObservableCollection<LoginModel> Logins { get; set; }

    public ICommand Add { get; set; }

    public string InputText
    {
      get { return inputText; }
      set
      {
        inputText = value;
        OnPropertyChanged();
      }
    }

    public override void Load()
    {
      base.Load();

      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
      {
        this.Logins.Add(new LoginModel(setting));
      }
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        window.ShowDialog();
      }
    }

    public void Close()
    {
      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        window.Close();
      }
    }

    public AddNewModel()
    {
      this.Add = new AddSelected(this);
      this.Logins = new ObservableCollection<LoginModel>();
    }
  }
}