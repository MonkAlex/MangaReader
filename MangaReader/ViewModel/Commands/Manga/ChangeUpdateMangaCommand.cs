using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ChangeUpdateMangaCommand : MultipleMangasBaseCommand
  {
    protected readonly bool NeedUpdate;

    public override void Execute(IEnumerable<IManga> mangas)
    {
      using (var context = Repository.GetEntityContext())
      {
        foreach (var manga in mangas.Where(m => m.NeedUpdate == NeedUpdate))
        {
          manga.NeedUpdate = !manga.NeedUpdate;
          context.Save(manga).GetAwaiter().GetResult();
        }
      }
    }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Any(m => m.NeedUpdate == NeedUpdate);
    }

    public ChangeUpdateMangaCommand(bool needUpdate, MainPageModel model) : base(model)
    {
      CanExecuteNeedSelection = true;
      this.NeedUpdate = needUpdate;
      this.Name = needUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update;
      this.Icon = needUpdate ? "pack://application:,,,/Icons/Manga/not_update.png" : "pack://application:,,,/Icons/Manga/need_update.png";
    }
  }
}