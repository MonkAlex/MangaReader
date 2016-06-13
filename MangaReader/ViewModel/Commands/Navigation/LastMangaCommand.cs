using System.Windows.Data;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Navigation
{
  public class LastMangaCommand : NavigationCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !IsLast;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      View.MoveCurrentToLast();
    }

    public LastMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "В конец";
      this.Icon = "pack://application:,,,/Icons/Navigation/end.png";
    }
  }
}