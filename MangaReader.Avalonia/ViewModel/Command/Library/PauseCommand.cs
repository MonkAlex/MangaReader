using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class PauseCommand : LibraryBaseCommand
  {

    public override bool CanExecute(object parameter)
    {
      return !Library.IsPaused && !Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Library.IsPaused = true;
    }

    public PauseCommand(LibraryViewModel model) : base(model)
    {
      this.Name = "Пауза";
      this.Icon = "pack://application:,,,/Icons/Main/pause.png";
    }
  }
}