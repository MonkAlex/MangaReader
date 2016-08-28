using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace Grouple
{
  /// <summary>
  /// Манга.
  /// </summary>
  public abstract class GroupleManga : Mangas
  {

    private static Parser parser = new Parser();

    #region Свойства

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
      if (!page.HasContent)
        return;

      // Если сайт ответил с другого адреса - переписываем текущий адрес.
      if (page.ResponseUri != this.Uri)
      {
        this.Uri = page.ResponseUri;
        this.Refresh();
        return;
      }

      // Если на странице редирект - выполняем его и получаем новую ссылку на мангу.
      if (page.Content.ToLowerInvariant().Contains(Parser.CookieKey))
      {
        var newUri = Parser.GetRedirectUri(page);
        if (!this.Uri.Equals(newUri))
        {
          this.Uri = newUri;
          this.Refresh();
          return;
        }
      }

      parser.UpdateNameAndStatus(this);
      OnPropertyChanged(nameof(IsCompleted));
    }

    protected override void UpdateContent()
    {
      this.Volumes.Clear();
      this.Chapters.Clear();
      this.Pages.Clear();

      parser.UpdateContent(this);

      base.UpdateContent();
    }
    
    #endregion
  }
}
