using System;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateAllCommand : BaseCommand
  {
    public override string Name { get { return Strings.Manga_Action_Update; } }

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      Library.ThreadAction(() => Library.Update(Library.LibraryMangas, ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription));
    }

    public UpdateAllCommand()
    {
      Library.UpdateCompleted += LibraryChanged;
      Library.UpdateStarted += LibraryChanged;
    }

    private void LibraryChanged(object sender, EventArgs eventArgs)
    {
      OnCanExecuteChanged();
    }
  }
}