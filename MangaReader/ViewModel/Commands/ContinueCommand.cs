using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ContinueCommand : BaseCommand
  {
    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && Library.IsPaused && !Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Library.IsPaused = false;
    }

    public ContinueCommand()
    {
      this.Name = Strings.Manga_Action_Restore;
      Library.PauseChanged += (o, a) => this.OnCanExecuteChanged();
      Library.AvaibleChanged += (o, a) => this.OnCanExecuteChanged();
    }
  }
}