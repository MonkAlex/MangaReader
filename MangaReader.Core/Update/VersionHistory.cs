using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MangaReader.Core.Update
{
  public static class VersionHistory
  {
    private static readonly Assembly Assembly = typeof(VersionHistory).Assembly;

    public static string GetHistory()
    {
      var text = string.Empty;
      var history = Assembly.GetManifestResourceNames().SingleOrDefault(r => r.EndsWith("Update.CHANGELOG.md"));
      if (history == null)
        return text;

      using (var reader = new StreamReader(Assembly.GetManifestResourceStream(history)))
      {
        text = reader.ReadToEnd();
      }
      return text;
    }

    public static Version GetVersion()
    {
      return Assembly.GetName().Version;
    }
  }
}