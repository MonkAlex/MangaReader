using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Manga
{
  public class MangaName
  {
    private string english;
    private string russian;
    private string japanese;

    /// <summary>
    /// Английское название манги.
    /// </summary>
    public string English
    {
      get
      {
        return !string.IsNullOrEmpty(this.english) ? english : Japanese;
      }
      set
      {
        english = value;
      }
    }

    /// <summary>
    /// Русское название манги.
    /// </summary>
    public string Russian
    {
      get
      {
        return !string.IsNullOrEmpty(this.russian) ? russian : Japanese;
      }
      set
      {
        russian = value;
      }
    }

    /// <summary>
    /// Японское название манги.
    /// </summary>
    public string Japanese
    {
      get
      {
        if (!string.IsNullOrEmpty(this.japanese))
          return japanese;
        if (!string.IsNullOrEmpty(this.english))
          return english;
        return !string.IsNullOrEmpty(this.russian) ? russian : string.Empty;
      }
      set
      {
        japanese = value;
      }
    }

    /// <summary>
    /// Определение строкового значения по настройкам.
    /// </summary>
    /// <returns>Название манги.</returns>
    public override string ToString()
    {
      if (ConfigStorage.Instance.AppConfig.Language == Languages.English)
        return English;
      if (ConfigStorage.Instance.AppConfig.Language == Languages.Russian)
        return Russian;
      return Japanese;
    }
  }
}
