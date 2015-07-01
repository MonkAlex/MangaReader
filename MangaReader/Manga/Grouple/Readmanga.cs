using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.Manga.Grouple
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Readmanga : Mangas
  {
    #region Свойства

    public new static Guid Type { get { return Guid.Parse("2C98BBF4-DB46-47C4-AB0E-F207E283142D"); } }

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public override bool IsCompleted
    {
      get
      {
        if (this.Status == null)
          return false;
        var match = Regex.Match(this.Status, Strings.Manga_IsCompleted);
        return match.Groups.Count > 1 && match.Groups[1].Value.Trim() == "завершен";
      }
    }

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Manga)).ToList(); }
    }

    /// <summary>
    /// Признак наличия глав.
    /// </summary>
    public override bool HasChapters
    {
      get { return true; }
      set { }
    }

    /// <summary>
    /// Признак наличия томов.
    /// </summary>
    public override bool HasVolumes
    {
      get { return true; }
      set { }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      var page = Page.GetPage(this.Uri);
      if (string.IsNullOrWhiteSpace(page))
        return;

      // Если на странице редирект - выполняем его и получаем новую ссылку на мангу.
      if (page.ToLowerInvariant().Contains(Getter.CookieKey))
      {
        var newUri = Getter.GetRedirectUri(this.Uri, page);
        if (!this.Uri.Equals(newUri))
        {
          this.Uri = newUri;
          this.Refresh();
          return;
        }
      }

      var newName = Getter.GetMangaName(page).ToString();
      if (string.IsNullOrWhiteSpace(newName))
        Log.Add("Не удалось получить имя манги, текущее название = " + this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;

      this.Status = Getter.GetTranslateStatus(page);
      OnPropertyChanged("IsCompleted");
    }

    protected override void UpdateContent()
    {
      this.Volumes.Clear();
      this.Chapters.Clear();
      this.Pages.Clear();

      var rmVolumes = Getter.GetLinksOfMangaChapters(Page.GetPage(this.Uri), this.Uri)
        .Select(cs => new Chapter(cs.Key, cs.Value))
        .GroupBy(c => c.Volume)
        .Select(g =>
        {
          var v = new Volume(g.Key);
          v.Chapters.AddRange(g);
          return v;
        });

      this.Volumes.AddRange(rmVolumes);
      base.UpdateContent();
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Открыть мангу.
    /// </summary>
    /// <param name="url">Ссылка на мангу.</param>
    public Readmanga(Uri url)
      : this()
    {
      this.Uri = url;
      this.Refresh();
    }

    public Readmanga()
      : base()
    {
      //this.CompressionMode = Compression.CompressionMode.Volume;
    }

    #endregion
  }
}
