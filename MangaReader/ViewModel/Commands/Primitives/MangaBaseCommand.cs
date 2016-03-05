using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class MangaBaseCommand : LibraryBaseCommand
  {
    protected readonly ListCollectionView View;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var manga = parameter as Mangas;
      if (manga != null)
      {
        this.Execute(manga);
        View.Refresh();
        OnCanExecuteChanged();
      }
      else
      {
        Log.Add("Cannot run manga command", parameter);
      }
    }

    public virtual void Execute(Mangas manga)
    {
      
    }

    public MangaBaseCommand(ListCollectionView view)
    {
      this.View = view;
    }
  }
}