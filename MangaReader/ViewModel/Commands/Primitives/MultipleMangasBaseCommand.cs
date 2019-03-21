using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public abstract class MultipleMangasBaseCommand : LibraryBaseCommand
  {
    private bool canExecuteNeedSelection;
    protected bool NeedRefresh { get; set; }

    protected bool CanExecuteNeedSelection
    {
      get { return canExecuteNeedSelection; }
      set
      {
        canExecuteNeedSelection = value;
        OnPropertyChanged();
        SubscribeToSelection(canExecuteNeedSelection);
      }
    }

    protected MainPageModel PageModel { get; }

    protected IEnumerable<MangaModel> SelectedModels => PageModel.SelectedMangaModels;

    public override async Task Execute(object parameter)
    {
      using (var context = Repository.GetEntityContext($"Manga command '{this.Name}'"))
      {
        var ids = SelectedModels.Select(m => m.Id).ToList();
        var query = await context.Get<IManga>().Where(m => ids.Contains(m.Id)).ToListAsync().ConfigureAwait(true);
        var mangas = query.OrderBy(m => ids.IndexOf(m.Id)).ToList();
        try
        {
          foreach (var model in SelectedModels)
            model.ContextManga = mangas.SingleOrDefault(m => m.Id == model.Id);
          await this.Execute(mangas).ConfigureAwait(true);
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
        PageModel.View.Refresh();

      foreach (var command in PageModel.MangaMenu.Select(m => m.Command).Where(m => m.GetType() == GetType()).OfType<MultipleMangasBaseCommand>())
        command.OnCanExecuteChanged();
    }

    public abstract Task Execute(IEnumerable<IManga> mangas);

    private void SubscribeToSelection(bool subscribe)
    {
      if (subscribe)
        PageModel.SelectedMangaModels.CollectionChanged += SelectedMangaModelsOnCollectionChanged;
      else
        PageModel.SelectedMangaModels.CollectionChanged -= SelectedMangaModelsOnCollectionChanged;
      OnCanExecuteChanged();
    }

    private void SelectedMangaModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      OnCanExecuteChanged();
    }

    protected MultipleMangasBaseCommand(MainPageModel model) : base(model.Library)
    {
      PageModel = model;
      this.NeedRefresh = true;
      this.CanExecuteNeedSelection = false;
    }
  }
}