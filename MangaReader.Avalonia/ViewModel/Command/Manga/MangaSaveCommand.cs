using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Core.Services.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class MangaSaveCommand : LibraryBaseCommand
  {
    private readonly INavigator navigator;
    private MangaModel model;

    private bool inProcess = false;

    public override bool CanExecute(object parameter)
    {
      if (parameter is MangaModel inputModel)
      {
        this.model = inputModel;
        this.Name = model.Saved ? "Save" : "Subscribe";
      }
      return !inProcess && (base.CanExecute(parameter) || !model.Saved);
    }

    public override async Task Execute(object parameter)
    {
      try
      {
        if (parameter is MangaModel inputModel)
          this.model = inputModel;

        inProcess = true;
        this.OnCanExecuteChanged();
        this.Name = "Process...";
        using (var context = Repository.GetEntityContext())
        {
          if (model.Saved)
          {
            var manga = await context.Get<IManga>().FirstAsync(m => m.Id == model.Id).ConfigureAwait(true);
            if (model.MangaName != manga.ServerName)
            {
              var name = model.MangaName;
              manga.IsNameChanged = true;
              manga.Name = name;
            }
            else
              manga.IsNameChanged = false;

            if (model.CompressionMode != null && model.CompressionModes.Contains(model.CompressionMode.Value))
              manga.CompressionMode = model.CompressionMode;

            manga.NeedCompress = model.NeedCompress;

            if (Uri.TryCreate(model.Uri, UriKind.Absolute, out Uri parsedUri) && parsedUri != manga.Uri)
              manga.Uri = parsedUri;

            await context.Save(manga).ConfigureAwait(true);
            model.UpdateProperties(manga);
          }
          else
          {
            var added = await Library.Add(new Uri(model.Uri)).ConfigureAwait(true);
            if (added.Success)
            {
              added.Manga.Cover = model.Cover;
              await context.Save(added.Manga).ConfigureAwait(true);
            }
          }
        }
        await navigator.OpenLibrary().ConfigureAwait(true);
      }
      catch (MangaReaderException ex)
      {
        Log.Exception(ex);
        if (model.Saved)
          using (var context = Repository.GetEntityContext())
          {
            var manga = await context.Get<IManga>().FirstAsync(m => m.Id == model.Id).ConfigureAwait(true);
            model.UpdateProperties(manga);
          }
      }
      finally
      {
        inProcess = false;
        this.OnCanExecuteChanged();
        this.Name = model.Saved ? "Save" : "Subscribe";
      }
    }

    public MangaSaveCommand(LibraryViewModel library, INavigator navigator) : base(library)
    {
      this.navigator = navigator;
    }
  }
}
