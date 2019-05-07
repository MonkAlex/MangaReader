using System;
using System.Diagnostics;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MangaReader.Core.Services
{
  public class Log
  {
    private static Lazy<Log> instance = new Lazy<Log>(() => new Log());

    internal ILogger Logger;

    public static event Action<LogEventStruct> LogReceived;

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Add(string message)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Debug(message);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Строка с форматированием.</param>
    /// <param name="args">Параметры форматирования.</param>
    public static void AddFormat(string message, params object[] args)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Debug(message, args);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Info(string message)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Info(message);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Строка с форматированием.</param>
    /// <param name="args">Параметры форматирования.</param>
    public static void InfoFormat(string message, params object[] args)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Info(message, args);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    public static void Exception(System.Exception ex)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Error(ex);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="message">Сообщение.</param>
    public static void Exception(System.Exception ex, string message)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.Logger.Error(ex, message);
      });
    }

    /// <summary>
    /// Добавление записи в лог.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public static void Exception(string message)
    {
      WriteExceptionToEventSource(() =>
      {
        var stack = new StackTrace(1);
        var content = string.Concat(message, Environment.NewLine, stack.ToString());
        instance.Value.Logger.Error(content);
      });
    }

    public static void Separator(string message)
    {
      WriteExceptionToEventSource(() =>
      {
        instance.Value.SeparatorImpl(message);
      });
    }

    private void SeparatorImpl(string message)
    {
      WriteExceptionToEventSource(() =>
      {
        Logger.Debug(string.Empty);
        Logger.Debug($"            *** {message} ***           ");
        Logger.Debug(string.Empty);
      });
    }

    private static void WriteExceptionToEventSource(Action action)
    {
      try
      {
        action();
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e);
      }
    }

    private Log()
    {
      var config = new LoggingConfiguration();
      var layout = @"${longdate} ${processid} ${threadid} ${assembly-version:MangaReader.Core} ${level} ${message}${onexception:${newline}${exception:format=tostring}}";

      var fileTarget = new FileTarget();
      config.AddTarget("file", fileTarget);
      fileTarget.FileName = @"${basedir}/${processname}.log";
      fileTarget.ArchiveFileName = "${basedir}/logs/${processname}.{#}.log";
      fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
      fileTarget.ArchiveEvery = FileArchivePeriod.Day;
      fileTarget.ArchiveOldFileOnStartup = true;
      fileTarget.CreateDirs = true;
      fileTarget.Layout = layout;
      fileTarget.Encoding = Encoding.UTF8;
      var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
      config.LoggingRules.Add(rule);

      var consoleTarget = new ColoredConsoleTarget();
      config.AddTarget("console", consoleTarget);
      consoleTarget.Layout = layout;
      var rule2 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
      config.LoggingRules.Add(rule2);

      var viewerTarget = new InfoTarget();
      config.AddTarget("viewer", viewerTarget);
      var rule3 = new LoggingRule("*", LogLevel.Info, viewerTarget);
      config.LoggingRules.Add(rule3);

      LogManager.Configuration = config;
      LogManager.ThrowExceptions = true;
      Logger = LogManager.GetLogger("default");
      SeparatorImpl("Initialized");
    }

    private class InfoTarget : Target
    {
      protected override void Write(NLog.Common.AsyncLogEventInfo logEvent)
      {
        base.Write(logEvent);

        LogReceived?.Invoke(new LogEventStruct(logEvent.LogEvent));
      }
    }
  }
}
