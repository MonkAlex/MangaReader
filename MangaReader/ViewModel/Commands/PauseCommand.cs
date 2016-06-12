using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class PauseCommand : BaseCommand
  {

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !Library.IsPaused && !Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Library.IsPaused = true;
    }

    public PauseCommand()
    {
      this.Name = Strings.Manga_Action_Pause;
      this.Icon = "pack://application:,,,/Icons/Main/pause.png";
      Library.PauseChanged += (o, a) => this.OnCanExecuteChanged();
      Library.AvaibleChanged += (o, a) => this.OnCanExecuteChanged();
    }
  }
}