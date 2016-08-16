namespace MangaReader.CLI
{
  class Program
  {
    static void Main(string[] args)
    {
      Core.Loader.Init();
      Initialize.Run();
    }
  }
}
