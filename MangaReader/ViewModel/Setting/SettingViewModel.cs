using System.Threading.Tasks;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public abstract class SettingViewModel : BaseViewModel
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

    public abstract Task Save();
  }
}