using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel
{
  public class MangaViewModel : ViewModelBase
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

    public MangaViewModel(IManga manga)
    {
      this.name = manga.Name;
      this.uri = manga.Uri;
      this.cover = manga.Cover;
    }
  }
}
