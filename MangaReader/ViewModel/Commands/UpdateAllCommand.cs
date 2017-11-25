using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateAllCommand : LibraryBaseCommand
  {

    public override async void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
        await Library.ThreadAction(() => Library.Update());
    }

    public UpdateAllCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Update;
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}