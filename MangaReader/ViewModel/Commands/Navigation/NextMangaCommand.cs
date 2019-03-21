using System.Threading.Tasks;
using System.Windows.Data;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Navigation
{
  public class NextMangaCommand : NavigationCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !IsLast;
    }

    public override Task Execute(object parameter)
    {
      View.MoveCurrentToNext();
      if (View.IsCurrentAfterLast)
        View.MoveCurrentToLast();

      return Task.CompletedTask;
    }

    public NextMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "Вперед";
      this.Icon = "pack://application:,,,/Icons/Navigation/next.png";
    }
  }
}