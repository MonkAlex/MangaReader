using System;
using System.Diagnostics;
using System.IO;

namespace MangaReader.Services
{
  public class Log
  {
    /// <summary>
    /// Указатель блокировки.
    /// </summary>
    private static readonly object LogLock = new object();

    /// <summary>
    /// Ссылка на файл лога.
    /// </summary>
    private static readonly string LogPath = Settings.WorkFolder + @".\manga.log";

    /// <summary>
    /// Ссылка на файл лога исключений.
    /// </summary>
    private static readonly string ExceptionPath = Settings.WorkFolder + @".\error.log";

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="messages">Сообщение.</param>
    public static void Add(params object[] messages)
    {
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", string.Join(Environment.NewLine, messages), Environment.NewLine);
      Write(contents, LogPath);
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="messages">Сообщение.</param>
    public static void Exception(Exception ex, params string[] messages)
    {
      var contents = string.Concat(DateTime.Now.ToString("O"), "   ", string.Join(Environment.NewLine, messages),
        Environment.NewLine, ex, Environment.NewLine);
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
        Environment.NewLine, stack.ToString(), Environment.NewLine);
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

    public Log()
    {
      throw new Exception("Use static methods. Dont create log object.");
    }
  }
}
