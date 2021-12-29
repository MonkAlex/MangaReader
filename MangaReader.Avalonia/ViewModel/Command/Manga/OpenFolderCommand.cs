using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using LibraryViewModel = MangaReader.Core.Services.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenFolderCommand : MultipleMangasBaseCommand
  {
    private readonly OpenFolderCommandBase baseCommand;

    public override Task Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
        baseCommand.Execute(m);

      return Task.CompletedTask;
    }

    public override bool CanExecute(object parameter)
    {
      return baseCommand.CanExecute(parameter) && CanExecuteMangaCommand();
    }

    public OpenFolderCommand(SelectionModel mangaModels, LibraryViewModel library) : base(mangaModels, library)
    {
      this.baseCommand = new OpenFolderCommandBase();
      this.Name = baseCommand.Name;
      this.Icon = baseCommand.Icon;
    }
  }
}
