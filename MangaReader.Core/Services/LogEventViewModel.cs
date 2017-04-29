using System.Globalization;
using NLog;

namespace MangaReader.Core.Services
{
  public struct LogEventStruct
  {
    public LogEventStruct(LogEventInfo logEventInfo)
    {
      ToolTip = logEventInfo.FormattedMessage;
      Level = logEventInfo.Level.ToString();
      FormattedMessage = logEventInfo.FormattedMessage;
      Time = logEventInfo.TimeStamp.ToString(CultureInfo.InvariantCulture);
    }


    public string Time { get; }
    public string Level { get; }
    public string FormattedMessage { get; }
    public string ToolTip { get; }
  }
}