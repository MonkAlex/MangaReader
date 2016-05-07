using System;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;
using MangaReader.Manga;
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
        Library.ThreadAction(() => Library.Update(Repository.Get<Mangas>(), ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
      }
    }

    public UpdateAllCommand()
    {
      this.Name = Strings.Manga_Action_Update;
    }
  }
}