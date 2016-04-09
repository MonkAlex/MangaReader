using MangaReader.Core.Update;
using MangaReader.Properties;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class VersionHistoryModel : BaseViewModel
  {
    private string history;
    private string version;

    public string History
    {
      get { return history; }
      set
      {
        history = value;
        OnPropertyChanged();
      }
    }

    public string Version
    {
      get { return version; }
      set
      {
        version = value;
        OnPropertyChanged();
      }
    }

    public override void Load()
    {
      base.Load();

      this.History = VersionHistory.GetHistory();
      this.Version = string.Format(Strings.Update_Label_Version, VersionHistory.GetVersion().ToString(3));
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet(this);
      if (window != null)
        window.ShowDialog();
    }
  }
}