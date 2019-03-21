using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ContinueCommand : LibraryBaseCommand
  {
    public override bool CanExecute(object parameter)
    {
      return Library.IsPaused && !Library.IsAvaible;
    }

    public override Task Execute(object parameter)
    {
      Library.IsPaused = false;
      return Task.CompletedTask;
    }

    public ContinueCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Restore;
      this.Icon = "pack://application:,,,/Icons/Main/play.png";
    }
  }
}