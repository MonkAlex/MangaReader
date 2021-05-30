using System;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command
{
  public class ChangelogNavigationCommand : BaseCommand
  {
    private const string RepoUri = "https://github.com/MonkAlex/MangaReader/path_to_remove";

    public override Task Execute(object parameter)
    {
      var uri = new Uri($"{RepoUri}{parameter}");
      Helper.StartUseShell(uri.ToString());
      return Task.CompletedTask;
    }
  }
}
