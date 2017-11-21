using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace MangaReader.Core.ApplicationControl
{
  public static class Server
  {
    public static void Run(string uniqueId)
    {
#warning Avalonia:Пайпы не хотят работать под моно.
      using (var server = new NamedPipeServerStream(uniqueId, PipeDirection.InOut,
        NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.None))
      {
        server.WaitForConnection();
        Task.Run(() => Run(uniqueId));
        using (var reader = new StreamReader(server))
        {
          while (!reader.EndOfStream)
          {
            var line = reader.ReadLine();
            Core.Client.OnOtherAppRunning(line);
          }
        }
      }
    }
  }
}