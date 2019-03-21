using System.Threading.Tasks;
using System.Windows.Data;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Navigation
{
  public class PreviousMangaCommand : NavigationCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !IsFirst;
    }

    public override Task Execute(object parameter)
    {
      View.MoveCurrentToPrevious();
      if (View.IsCurrentBeforeFirst)
        View.MoveCurrentToFirst();

      return Task.CompletedTask;
    }

    public PreviousMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "Назад";
      this.Icon = "pack://application:,,,/Icons/Navigation/previous.png";
    }
  }
}