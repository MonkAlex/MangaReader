using System.Windows;
using MangaReader.Core.Exception;
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

        manga.Save();
        model.UpdateProperties(manga);
        model.Close();
      }
      catch (MangaReaderException ex)
      {
        MessageBox.Show(ex.Message);
        model.ContextManga.Update();
        model.UpdateProperties(model.ContextManga);
      }
    }

    public MangaSaveCommand(MangaModel model, LibraryViewModel library) : base(library)
    {
      this.model = model;
      this.Name = "Принять";
    }
  }
}