using System;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class DelegateCommand : BaseCommand
  {
    protected Func<Task> taskExecute;
    protected Action actionExecute;
    protected Func<bool> canExecute;
    
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && canExecute.Invoke();
    }

    public override async Task Execute(object parameter)
    {
      try
      {
        if (taskExecute != null)
          await taskExecute.Invoke().ConfigureAwait(false);
        else
          actionExecute.Invoke();
      }
      catch (Exception e)
      {
        Log.Exception(e);
      }
    }

    public DelegateCommand(Action execute)
    {
      this.actionExecute = execute;
      this.canExecute = () => true;
    }

    public DelegateCommand(Func<Task> execute, Func<bool> canExecute)
    {
      this.taskExecute = execute;
      this.canExecute = canExecute;
    }
  }
}