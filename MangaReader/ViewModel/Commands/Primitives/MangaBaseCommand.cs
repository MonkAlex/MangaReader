using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class MangaBaseCommand : LibraryBaseCommand
  {
    protected readonly ListCollectionView View;

    protected bool NeedRefresh { get; set; }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var manga = parameter as MangaViewModel;
      if (manga != null && manga.Manga != null)
      {
        this.Execute(manga.Manga);
#warning Blazard - disable refresh
        /*
        if (NeedRefresh)
          View.Refresh();
          */
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

    public MangaBaseCommand(ListCollectionView view)
    {
      this.View = view;
      this.NeedRefresh = true;
    }
  }
}