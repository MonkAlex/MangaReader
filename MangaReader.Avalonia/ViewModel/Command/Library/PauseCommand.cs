using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Library
{
  public class PauseCommand : LibraryBaseCommand
  {

    public override bool CanExecute(object parameter)
    {
      return !Library.IsPaused && !Library.IsAvaible;
    }

    public override Task Execute(object parameter)
    {
      Library.IsPaused = true;
      return Task.CompletedTask;
    }

    public PauseCommand(LibraryViewModel model) : base(model)
    {
      this.Name = "Пауза";
      this.Icon = "pack://application:,,,/Icons/Main/pause.png";
    }
  }
}