using System;
using MangaReader.Core;
using MangaReader.Services;

namespace MangaReader.CLI
{
  class Program
  {
    static void Main(string[] args)
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
