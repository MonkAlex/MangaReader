using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class SettingViewModel : BaseViewModel
  {
    private string header;

    public string Header
    {
      get { return header; }
      set
      {
        header = value;
        OnPropertyChanged();
      }
    }

    public virtual void Save()
    {

    }
  }
}