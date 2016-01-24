using System;
using MangaReader.Core;
using MangaReader.Services;

namespace MangaReader.CLI
{
  public static class Initialize
  {
    public static void Run()
    {
      Client.Init();
      Client.Start(new ConsoleProgress());

      foreach (var manga in Library.LibraryMangas)
      {
        Console.WriteLine("{0}:{1}", manga.Id, manga);
      }
    }
  }
}