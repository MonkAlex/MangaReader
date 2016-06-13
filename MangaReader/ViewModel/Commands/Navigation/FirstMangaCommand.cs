using System.Windows.Data;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Navigation
{
  public class FirstMangaCommand : NavigationCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !IsFirst;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      View.MoveCurrentToFirst();
    }

    public FirstMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "В начало";
      this.Icon = "pack://application:,,,/Icons/Navigation/skip_to_start.png";
    }
  }
}