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
    private readonly MangaModel model;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) || !model.Saved;
    }

    public override async Task Execute(object parameter)
    {
      try
      {
        using (var context = Repository.GetEntityContext())
        {
          if (model.Saved)
          {
            var manga = context.Get<IManga>().First(m => m.Id == model.Id);
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
            foreach (var viewModel in ExplorerViewModel.Instance.Tabs.OfType<Explorer.LibraryViewModel>())
            {
              var added = await viewModel.Library.Add(new Uri(model.Uri)).ConfigureAwait(true);
              if (added.Success)
              {
                added.Manga.Cover = model.Cover;
                await context.Save(added.Manga).ConfigureAwait(true);
              }
            }
          }
        }
        ExplorerViewModel.Instance.SelectedTab = ExplorerViewModel.Instance.Tabs.OfType<Explorer.LibraryViewModel>().FirstOrDefault();
      }
      catch (MangaReaderException ex)
      {
        Log.Exception(ex);
        if (model.Saved)
          using (var context = Repository.GetEntityContext())
          {
            var manga = context.Get<IManga>().First(m => m.Id == model.Id);
            model.UpdateProperties(manga);
          }
      }
    }

    public MangaSaveCommand(MangaModel model, LibraryViewModel library) : base(library)
    {
      this.model = model;
      this.Name = "Принять";
    }
  }
}