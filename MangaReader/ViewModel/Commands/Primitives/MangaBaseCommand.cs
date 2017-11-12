using System;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class MangaBaseCommand : LibraryBaseCommand
  {
    protected bool NeedRefresh { get; set; }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      if (parameter is MangaModel model)
      {
        using (var context = Repository.GetEntityContext())
        {
          var manga = context.Get<IManga>().Single(m => m.Id == model.Id);
          try
          {
            model.ContextManga = manga;
            this.Execute(manga);

          }
          catch (Exception e)
          {
            Log.Exception(e);
          }
          finally
          {
            model.UpdateProperties(manga);
            model.ContextManga = null;
          }
        }

        if (NeedRefresh)
          WindowModel.Instance.Refresh();

        OnCanExecuteChanged();
      }
      else
      {
        Log.AddFormat("Cannot run manga command '{0}' with parameter '{1}'.", this, parameter);
      }
    }

    public virtual void Execute(IManga manga)
    {

    }

    public MangaBaseCommand(LibraryViewModel model) : base(model)
    {
      this.NeedRefresh = true;
    }
  }
}