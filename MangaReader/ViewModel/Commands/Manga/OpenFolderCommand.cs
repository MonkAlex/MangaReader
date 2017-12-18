using System.Collections.Generic;
using MangaReader.Core.Manga;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommand : MultipleMangasBaseCommand
  {
    private readonly OpenFolderCommandBase baseCommand;

    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
        baseCommand.Execute(m);
    }

    public override bool CanExecute(object parameter)
    {
      return baseCommand.CanExecute(parameter);
    }

    public OpenFolderCommand(MainPageModel model) : base(model)
    {
      this.baseCommand = new OpenFolderCommandBase();
      this.Name = baseCommand.Name;
      this.Icon = baseCommand.Icon;
      this.NeedRefresh = false;
    }
  }
}