using System.Collections.Generic;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenFolderCommand : MultipleMangasBaseCommand
  {
    private readonly OpenFolderCommandBase baseCommand;

    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
        baseCommand.Execute(m);
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