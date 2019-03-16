using System;
using System.Windows;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class MangaSaveCommand : LibraryBaseCommand
  {
    private readonly MangaModel model;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      try
      {
        var manga = model.ContextManga;
        using (var context = Repository.GetEntityContext())
        {
          if (model.CanChangeName)
          {
            var name = model.Name;
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

          context.Save(manga).GetAwaiter().GetResult();
          model.UpdateProperties(manga);
        }
        model.Close();
      }
      catch (MangaReaderException ex)
      {
        MessageBox.Show(ex.Message);
        using (var context = Repository.GetEntityContext())
        {
          context.Refresh(model.ContextManga).GetAwaiter().GetResult();
          model.UpdateProperties(model.ContextManga);
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