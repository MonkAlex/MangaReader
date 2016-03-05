using System;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateAllCommand : LibraryBaseCommand
  {

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
      {
        Library.ThreadAction(() => Library.Update(Library.LibraryMangas, ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
      }
    }

    public UpdateAllCommand()
    {
      this.Name = Strings.Manga_Action_Update;
    }
  }
}