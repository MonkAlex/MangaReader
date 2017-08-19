using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class MangaCommand : BaseCommand
  {
    public override void Execute(object parameter)
    {
      var manga = parameter as IManga;
      if (manga == null)
        Log.AddFormat("Command runned not for manga, parameter = {0}", parameter);
      else
        Execute(manga);
    }

    public virtual void Execute(IManga manga)
    {
      
    }

    public override bool CanExecute(object parameter)
    {
      var manga = parameter as IManga;
      return manga != null && CanExecute(manga);
    }

    public virtual bool CanExecute(IManga manga)
    {
      return true;
    }
    
  }
}