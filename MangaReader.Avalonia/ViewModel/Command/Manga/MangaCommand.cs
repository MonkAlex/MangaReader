using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
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

    protected Explorer.LibraryViewModel LibraryModel { get; }

    protected IEnumerable<MangaModel> SelectedModels => LibraryModel.SelectedMangaModels;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Any();
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      using (var context = Repository.GetEntityContext($"Manga command '{this.Name}'"))
      {
        var ids = SelectedModels.Select(m => m.Id).ToList();
        var mangas = context.Get<IManga>().Where(m => ids.Contains(m.Id)).ToList().OrderBy(m => ids.IndexOf(m.Id)).ToList();
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
      {
        var selected = SelectedModels.ToList();
        LibraryModel.FilteredItems.Reset();
        foreach (var model in selected.Where(s => LibraryModel.FilteredItems.Contains(s) && !LibraryModel.SelectedMangaModels.Contains(s)))
          LibraryModel.SelectedMangaModels.Add(model);
      }

      foreach (var command in LibraryModel.Commands.Where(m => m.GetType() == GetType()).OfType<MultipleMangasBaseCommand>())
        command.OnCanExecuteChanged();
    }

    public abstract void Execute(IEnumerable<IManga> mangas);

    private void SubscribeToSelection(bool subscribe)
    {
      if (subscribe)
        LibraryModel.SelectedMangaModels.CollectionChanged += SelectedMangaModelsOnCollectionChanged;
      else
        LibraryModel.SelectedMangaModels.CollectionChanged -= SelectedMangaModelsOnCollectionChanged;
      OnCanExecuteChanged();
    }

    private void SelectedMangaModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      OnCanExecuteChanged();
    }

    protected MultipleMangasBaseCommand(Explorer.LibraryViewModel model) : base(model.Library)
    {
      LibraryModel = model;
      this.NeedRefresh = true;
      this.CanExecuteNeedSelection = true;
      this.CanExecuteChanged += OnCanExecuteChanged;
    }

    private void OnCanExecuteChanged(object sender, EventArgs eventArgs)
    {
      IsVisible = CanExecute(null);
    }
  }
}