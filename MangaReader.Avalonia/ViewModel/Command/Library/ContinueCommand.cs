using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class ContinueCommand : LibraryBaseCommand
  {
    public override bool CanExecute(object parameter)
    {
      return Library.IsPaused && !Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Library.IsPaused = false;
    }

    public ContinueCommand(LibraryViewModel model) : base(model)
    {
      this.Name = "Продолжить";
      this.Icon = "pack://application:,,,/Icons/Main/play.png";
    }
  }
}