using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel
{
  public class AddNewModel : BaseViewModel
  {
    public ObservableCollection<SettingViewModel> BookmarksModels { get; set; }

    public override void Load()
    {
      base.Load();

      var addFromUri = new AddFromUri(this);
      this.BookmarksModels.Add(addFromUri);
      var settings = Core.NHibernate.Repository.GetStateless<Core.Services.MangaSetting>().ToList();
      foreach (var setting in settings)
      {
        this.BookmarksModels.Add(new AddBookmarksModel(setting.Login, setting.MangaName, setting.Manga, addFromUri, this));
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

    public void Close()
    {
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.Close();
      }
    }

    public AddNewModel()
    {
      this.BookmarksModels = new ObservableCollection<SettingViewModel>();
    }
  }
}
