using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
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
      this.Name = Strings.Manga_Action_Pause;
      this.Icon = "pack://application:,,,/Icons/Main/pause.png";
    }
  }
}