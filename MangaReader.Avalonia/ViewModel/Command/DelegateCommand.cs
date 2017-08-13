using System;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class DelegateCommand : BaseCommand
  {
    protected Action execute;
    protected Func<bool> canExecute;
    
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && canExecute.Invoke();
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);
      
      execute.Invoke();
    }

    public void CanExecuteChanged()
    {
      this.OnCanExecuteChanged();
    }

    public DelegateCommand(Action execute) : this(execute, () => true)
    {
      
    }

    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }
  }
}