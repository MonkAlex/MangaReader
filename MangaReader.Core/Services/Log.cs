using System;
using System.Diagnostics;
using System.IO;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{
  public static class Log
  {
    /// <summary>
    /// Указатель блокировки.
    /// </summary>
    private static readonly object LogLock;

    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string LogPath = Path.Combine(ConfigStorage.WorkFolder, "manga.log");

    /// <summary>
    /// Ссылка на файл лога исключений.
    /// </summary>
    private static readonly string ExceptionPath = Path.Combine(ConfigStorage.WorkFolder, "error.log");

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Add(string message)
    {
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", message, Environment.NewLine);
      Write(contents, LogPath);
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="format">Строка с форматированием.</param>
    /// <param name="args">Параметры форматирования.</param>
    public static void AddFormat(string format, params object[] args)
    {
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", string.Format(format, args), Environment.NewLine);
      Write(contents, LogPath);
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="messages">Сообщение.</param>
    public static void Exception(System.Exception ex, params string[] messages)
    {
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", string.Join(Environment.NewLine, messages),
        Environment.NewLine, ex, Environment.NewLine, Environment.NewLine);
      Write(contents, ExceptionPath);
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="messages">Сообщение.</param>
    public static void Exception(params string[] messages)
    {
      var stack = new StackTrace(1);
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", string.Join(Environment.NewLine, messages),
        Environment.NewLine, stack.ToString(), Environment.NewLine, Environment.NewLine);
      Write(contents, ExceptionPath);
    }

    /// <summary>
    /// Записать в историю.
    /// </summary>
    /// <param name="contents">Сообщение.</param>
    /// <param name="path">Файл.</param>
    private static void Write(string contents, string path)
    {
      Console.WriteLine(contents);
      using (TimedLock.Lock(LogLock))
      {
        File.AppendAllText(path, contents, System.Text.Encoding.UTF8);
      }
    }

    static Log()
    {
      // Mono fix
      LogLock = "lc?";
    }
  }
}
