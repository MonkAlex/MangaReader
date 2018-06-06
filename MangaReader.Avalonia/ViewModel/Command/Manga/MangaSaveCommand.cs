using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Exception;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Core.Services.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
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
#warning Тут надо как то без окон обойтись
        // model.Close();
      }
      catch (MangaReaderException ex)
      {
        Log.Exception(ex);
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