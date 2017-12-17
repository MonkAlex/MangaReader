using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class MultipleMangasBaseCommand : LibraryBaseCommand
  {
    private IEnumerable<MangaModel> selectedModels;
    protected bool NeedRefresh { get; set; }

    protected IEnumerable<MangaModel> SelectedModels
    {
      get
      {
        if (selectedModels == null)
        {
          if (WindowModel.Instance.Content is FrameworkElement fe)
          {
            if (fe.DataContext is MainPageModel pageModel)
              selectedModels = pageModel.SelectedMangaModels;
          }
        }

        return selectedModels;
      }
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      using (var context = Repository.GetEntityContext())
      {
        var ids = SelectedModels.Select(m => m.Id).ToList();
        var mangas = context.Get<IManga>().Where(m => ids.Contains(m.Id)).ToList();
        try
        {
          foreach (var model in SelectedModels)
            model.ContextManga = mangas.SingleOrDefault(m => m.Id == model.Id);
          this.Execute(mangas);
        }
        catch (Exception e)
        {
          Log.Exception(e);
        }
        finally
        {
          foreach (var model in SelectedModels)
          {
            model.UpdateProperties(mangas.SingleOrDefault(m => m.Id == model.Id));
            model.ContextManga = null;
          }
        }
      }

      if (NeedRefresh)
        WindowModel.Instance.Refresh();

      OnCanExecuteChanged();
    }

    public virtual void Execute(IEnumerable<IManga> mangas)
    {

    }

    public MultipleMangasBaseCommand(LibraryViewModel model) : base(model)
    {
      this.NeedRefresh = true;
    }
  }
}