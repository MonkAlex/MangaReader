using System.IO;
using System.IO.Pipes;
using MangaReader.Core.Services;

namespace MangaReader.Core.ApplicationControl
{
  public static class Client
  {
    public static void Run(string uniqueId, Messages message)
    {
      using (var client = new NamedPipeClientStream(uniqueId))
      {
        client.Connect(5000);
        using (var writer = new StreamWriter(client))
        {
          writer.WriteLine(message.ToString("G"));
          writer.Flush();
          Log.Add($"Send command to server : {message}");
        }
      }
    }
  }
}