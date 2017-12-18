using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
    private bool isVisible;
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

    public bool IsVisible
    {
      get { return isVisible; }
      set
      {
        isVisible = value;
        OnPropertyChanged();
      }
    }

    protected MainPageModel PageModel { get; }

    protected IEnumerable<MangaModel> SelectedModels => PageModel.SelectedMangaModels;

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
        PageModel.View.Refresh();

      foreach (var command in PageModel.MangaMenu.Select(m => m.Command).Where(m => m.GetType() == GetType()).OfType<MultipleMangasBaseCommand>())
        command.OnCanExecuteChanged();
    }

    public abstract void Execute(IEnumerable<IManga> mangas);

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
      this.CanExecuteChanged += OnCanExecuteChanged;
    }

    private void OnCanExecuteChanged(object sender, EventArgs eventArgs)
    {
      IsVisible = CanExecute(null);
    }
  }
}