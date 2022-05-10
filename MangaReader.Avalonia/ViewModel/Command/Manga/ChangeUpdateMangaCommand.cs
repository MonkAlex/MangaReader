﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Avalonia.Properties;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class ChangeUpdateMangaCommand : MultipleMangasBaseCommand
  {
    protected readonly bool NeedUpdate;

    public override async Task Execute(IEnumerable<IManga> mangas)
    {
      using (var context = Repository.GetEntityContext())
      {
        foreach (var manga in mangas.Where(m => m.NeedUpdate == NeedUpdate))
        {
          manga.NeedUpdate = !manga.NeedUpdate;
          await context.Save(manga).ConfigureAwait(true);
        }
      }
    }

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && SelectedModels.Any(m => m.NeedUpdate == NeedUpdate);
    }

    public ChangeUpdateMangaCommand(bool needUpdate, SelectionModel mangaModels, LibraryViewModel library) 
      : base(mangaModels, library)
    {
      CanExecuteNeedSelection = true;
      this.NeedUpdate = needUpdate;
      this.Name = needUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update;
      this.Icon = needUpdate ? "pack://application:,,,/Icons/Manga/not_update.png" : "pack://application:,,,/Icons/Manga/need_update.png";
    }
  }
}
