using System.Windows;
using MangaReader.Properties;
using MangaReader.Update;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class VersionHistoryModel : BaseViewModel
  {
    private string history;
    private string version;
    protected internal Window window { get { return this.view as Window; } }

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

      window.ShowDialog();
    }

    public VersionHistoryModel(Window window) : base(window)
    {

    }
  }
}