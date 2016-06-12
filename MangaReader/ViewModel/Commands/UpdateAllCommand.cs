using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Properties;
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
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}