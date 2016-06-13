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

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      View.MoveCurrentToNext();
      if (View.IsCurrentAfterLast)
        View.MoveCurrentToLast();
    }

    public NextMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "Вперед";
      this.Icon = "pack://application:,,,/Icons/Navigation/next.png";
    }
  }
}