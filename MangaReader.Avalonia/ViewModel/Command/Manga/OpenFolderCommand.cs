using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;

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

    public OpenFolderCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.baseCommand = new OpenFolderCommandBase();
      this.Name = baseCommand.Name;
      this.Icon = baseCommand.Icon;
      this.NeedRefresh = false;
    }
  }
}