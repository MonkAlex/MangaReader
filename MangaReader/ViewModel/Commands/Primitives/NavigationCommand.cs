using System;
using System.Windows.Data;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class NavigationCommand : BaseCommand
  {
    protected ListCollectionView View;

    protected bool IsFirst { get; set; }

    protected bool IsLast { get; set; }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && View != null;
    }

    public NavigationCommand(ListCollectionView view)
    {
      this.View = view;
      this.View.CurrentChanged += ViewOnCurrentChanged;
      ViewOnCurrentChanged(this.View, EventArgs.Empty);
    }

    private void ViewOnCurrentChanged(object sender, EventArgs eventArgs)
    {
      var pos = this.View.CurrentPosition;
      IsFirst = pos == 0;
      IsLast = pos == (View.Count - 1);
      OnCanExecuteChanged();
    }
  }
}