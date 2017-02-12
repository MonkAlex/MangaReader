using System.Collections.ObjectModel;
using System.Linq;
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

    public ObservableCollection<AddBookmarksModel> BookmarksModels { get; set; }

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

      foreach (var settings in ConfigStorage.Instance.DatabaseConfig.MangaSettings
                                 .GroupBy(s => s.Login).Where(s => s.Key != null))
      {
        var name = string.Join(" \\ ", settings.Select(s => s.MangaName));
        this.BookmarksModels.Add(new AddBookmarksModel(settings.Key, name));
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
      this.Add = new AddSelected(this);
      this.BookmarksModels = new ObservableCollection<AddBookmarksModel>();
    }
  }
}