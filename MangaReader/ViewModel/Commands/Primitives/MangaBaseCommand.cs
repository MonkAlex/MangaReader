using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class MangaBaseCommand : LibraryBaseCommand
  {
    protected bool NeedRefresh { get; set; }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var manga = parameter as MangaViewModel;
      if (manga != null && manga.Manga != null)
      {
        this.Execute(manga.Manga);

        if (NeedRefresh)
          WindowModel.Instance.Refresh();

        OnCanExecuteChanged();
      }
      else
      {
        Log.AddFormat("Cannot run manga command '{0}' with parameter '{1}'.", this, parameter);
      }
    }

    public virtual void Execute(Mangas manga)
    {
      
    }

    public MangaBaseCommand()
    {
      this.NeedRefresh = true;
    }
  }
}