using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class ContentMenuItem : Primitive.NotifyPropertyChanged
  {
    private string name;
    private ICommand command;
    private ObservableCollection<ContentMenuItem> subItems;
    private FontWeight fontWeight;
    private bool isDefault;

    public FontWeight FontWeight
    {
      get { return fontWeight; }
      set
      {
        fontWeight = value;
        OnPropertyChanged();
      }
    }

    public bool IsDefault
    {
      get { return isDefault; }
      set
      {
        isDefault = value;
        this.FontWeight = value ? FontWeights.Bold : FontWeights.Normal;
        OnPropertyChanged();
      }
    }

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public ICommand Command
    {
      get { return command; }
      set
      {
        command = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> SubItems
    {
      get { return subItems; }
      set
      {
        subItems = value;
        OnPropertyChanged();
      }
    }

    public ContentMenuItem(BaseCommand command) : this(command, command.Name)
    {
    }

    public ContentMenuItem(ICommand command, string name) : this(name)
    {
      this.Command = command;
    }

    public ContentMenuItem(string name)
    {
      this.Name = name;
      this.SubItems = new ObservableCollection<ContentMenuItem>();
    }
  }
}