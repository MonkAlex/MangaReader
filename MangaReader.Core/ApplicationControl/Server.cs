﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.Core.Services;

namespace MangaReader.Core.ApplicationControl
{
  public static class Server
  {
    private static async Task Run(string uniqueId, CancellationToken token)
    {
      try
      {
        using (var server = new NamedPipeServerStream(uniqueId, PipeDirection.InOut,
          NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
        {
          await server.WaitForConnectionAsync(token).ConfigureAwait(false);
          RunTask(uniqueId, token);
          using (var reader = new StreamReader(server))
          {
            while (!reader.EndOfStream)
            {
              token.ThrowIfCancellationRequested();

              var line = await reader.ReadLineAsync().ConfigureAwait(false);
              Log.Add($"Server get command : {line}");
              Core.Client.OnOtherAppRunning(line);
            }
          }
        }
      }
      catch (OperationCanceledException ex) when (ex.CancellationToken == token && token.IsCancellationRequested) { }
    }

    public static Task RunTask(string uniqueId, CancellationToken token)
    {
      return Task.Run(() => Run(uniqueId, token), token);
    }
  }
}
