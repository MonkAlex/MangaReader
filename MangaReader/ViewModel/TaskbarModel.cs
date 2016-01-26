using System.Windows.Input;
using MangaReader.ViewModel.Commands;

namespace MangaReader.ViewModel
{
  public static class TaskbarModel
  {
    public static ICommand UpdateAll { get; set; }

    public static ICommand Close { get; set; }

    static TaskbarModel()
    {
      UpdateAll = new UpdateAllCommand();
      Close = new ExitCommand();
    }
  }
}