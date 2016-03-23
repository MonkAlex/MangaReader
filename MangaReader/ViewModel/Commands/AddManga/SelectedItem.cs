using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class SelectedItem<T> : NotifyPropertyChanged
  {
    public T Value { get; set; }

    private bool isSelected;

    public bool IsSelected
    {
      get { return isSelected; }
      set
      {
        isSelected = value;
        OnPropertyChanged();
      }
    }

    public SelectedItem(T value)
    {
      this.Value = value;
    } 
  }
}