using System.Drawing;
using System.Windows.Input;

namespace MangaReader.ViewModel.Commands
{
  public class ContentMenuItem : Primitive.NotifyPropertyChanged
  {
    private string name;
    private ICommand command;

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

    public ContentMenuItem(ICommand command, string name)
    {
      this.Command = command;
      this.Name = name;
    }

    public ContentMenuItem(BaseCommand command)
    {
      this.Command = command;
      this.Name = command.Name;
    }
  }
}