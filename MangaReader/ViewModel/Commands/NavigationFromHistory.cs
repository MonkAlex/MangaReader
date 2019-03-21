using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class NavigationFromHistory : BaseCommand
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