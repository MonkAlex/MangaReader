using System;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class UpdateWithPauseCommand : BaseCommand
  {
    private BaseCommand Update;
    private BaseCommand Pause;
    private BaseCommand Continue;
    private BaseCommand activeCommand;
    private Core.Services.LibraryViewModel library;

    private BaseCommand ActiveCommand
    {
      get { return activeCommand; }
      set
      {
        activeCommand = value;
        OnCanExecuteChanged();
        this.Name = activeCommand.Name;
        this.Icon = activeCommand.Icon;
      }
    }

    public override bool CanExecute(object parameter)
    {
      return ActiveCommand.CanExecute(parameter);
    }

    public override async Task Execute(object parameter)
    {
      if (ActiveCommand == Update)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          await ActiveCommand.Execute(parameter).ConfigureAwait(true);
          ActiveCommand = Pause;
        }
      }
      else if (ActiveCommand == Pause)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          await ActiveCommand.Execute(parameter).ConfigureAwait(true);
          ActiveCommand = Continue;
        }
      }
      else if (ActiveCommand == Continue)
      {
        if (ActiveCommand.CanExecute(parameter))
        {
          await ActiveCommand.Execute(parameter).ConfigureAwait(true);
          ActiveCommand = Pause;
        }
      }
    }

    public UpdateWithPauseCommand(LibraryViewModel view, Core.Services.LibraryViewModel library)
    {
      this.library = library;
      this.Update = new UpdateVisibleMangaCommand(view, library);
      this.Pause = new PauseCommand(library);
      this.Continue = new ContinueCommand(library);
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
        this.ActiveCommand = library.IsPaused ? this.Continue : this.Pause;
      }
    }
  }
}