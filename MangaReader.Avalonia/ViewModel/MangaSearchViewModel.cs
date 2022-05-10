using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel
{
  public class MangaSearchViewModel : ViewModelBase
  {
    private string name;
    private byte[] cover;
    private Uri uri;

    public string Name
    {
      get => name;
      set => RaiseAndSetIfChanged(ref name, value);
    }

    public byte[] Cover
    {
      get => cover;
      set => RaiseAndSetIfChanged(ref cover, value);
    }

    public Uri Uri
    {
      get => uri;
      set => RaiseAndSetIfChanged(ref uri, value);
    }

    public string Status
    {
      get => status;
      set => RaiseAndSetIfChanged(ref status, value);
    }

    private string status;

    public PreviewFoundMangaCommand PreviewFoundManga
    {
      get => previewFoundManga;
      set => RaiseAndSetIfChanged(ref previewFoundManga, value);
    }

    private PreviewFoundMangaCommand previewFoundManga;

    public MangaSearchViewModel(IManga manga, PreviewFoundMangaCommand previewFoundMangaCommand)
    {
      this.name = manga.Name;
      this.uri = manga.Uri;
      this.cover = manga.Cover;
      this.status = manga.Status;
      this.previewFoundManga = previewFoundMangaCommand;
    }
  }
}
