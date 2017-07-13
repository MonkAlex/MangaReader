using System.IO;
using System.IO.Pipes;

namespace MangaReader.Core.ApplicationControl
{
  public static class Client
  {
    public static void Run(string uniqueId, Messages message)
    {
#warning Пайпы не хотят работать под моно, пока отключаем.
      /*
      using (var client = new NamedPipeClientStream(uniqueId))
      {
        client.Connect(5000);
        using (var writer = new StreamWriter(client))
        {
          writer.WriteLine(message.ToString("G"));
          writer.Flush();
        }
      }
      */
    }
  }
}