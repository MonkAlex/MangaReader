using System;
using System.Linq;
using System.Reflection;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core
{
  /// <summary>
  /// Базовая реализация плагина для снижения дублирования кода.
  /// </summary>
  /// <remarks>Не обязательна к использованию.</remarks>
  public abstract class BasePlugin : IPlugin
  {
    public virtual string Name { get { return this.MangaType.Name; } }
    public abstract string ShortName { get; }
    public abstract Assembly Assembly { get; }
    public abstract Guid MangaGuid { get; }
    public abstract Type MangaType { get; }
    public abstract Type LoginType { get; }

    public virtual MangaSetting GetSettings()
    {
      return ConfigStorage.Instance.DatabaseConfig.MangaSettings.Single(m => Equals(m.Manga, this.MangaGuid));
    }

    public abstract ISiteParser GetParser();
    public abstract HistoryType HistoryType { get; }
  }
}