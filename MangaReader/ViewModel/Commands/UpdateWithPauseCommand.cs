using System;
using System.Windows.Data;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateWithPauseCommand : BaseCommand
  {
    private BaseCommand Update;
    private BaseCommand Pause;
    private BaseCommand Continue;
    private BaseCommand activeCommand;

    private BaseCommand ActiveCommand
    {
      get { return activeCommand; }
      set
      {
        activeCommand = value;
        OnCanExecuteChanged();
        this.Name = activeCommand.Name;
      }
    }

    public override bool CanExecute(object parameter)
    {
      return ActiveCommand.CanExecute(parameter);
    }

    public override void Execute(object parameter)
    {
      if (ActiveCommand == Update)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          ActiveCommand.Execute(parameter);
          ActiveCommand = Pause;
        }
      }
      else if (ActiveCommand == Pause)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          ActiveCommand.Execute(parameter);
          ActiveCommand = Continue;
        }
      }
      else if (ActiveCommand == Continue)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          ActiveCommand.Execute(parameter);
          ActiveCommand = Pause;
        }
      }
    }

    public UpdateWithPauseCommand(ListCollectionView view)
    {
      this.Update = new UpdateVisibleMangaCommand(view);
      this.Pause = new PauseCommand();
      this.Continue = new ContinueCommand();
      this.ActiveCommand = this.Update;
      this.Update.CanExecuteChanged += UpdateOnCanExecuteChanged;
    }

    private void UpdateOnCanExecuteChanged(object sender, EventArgs eventArgs)
    {
      if (this.Update.CanExecute(sender))
      {
        if (this.ActiveCommand != this.Update)
          this.ActiveCommand = this.Update;
      }
      else if (this.ActiveCommand == this.Update)
      {
        this.ActiveCommand = Library.IsPaused ? this.Continue : this.Pause;
      }
    }
  }
}