using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;

namespace Grouple
{
  public class GroupleMangaPage : MangaPage
  {
    public List<Uri> Servers { get; set; }

    protected override void OnDownloadFailed()
    {
      var baseImage = ImageLink.OriginalString;
      if (Servers == null || Servers.Count == 1)
        base.OnDownloadFailed();

      ImageLink = SelectNextServer(ImageLink);
      throw new Exception($"Restart download, downloaded file not found on {baseImage}, try to load from {ImageLink}");
    }

    private Uri SelectNextServer(Uri link)
    {
      var host = new Uri($"{link.Scheme}://{link.Host}");
      var index = Servers.IndexOf(host);
      if (index < 0)
        throw new Exception($"Grouple not support host {link.Host}");
      var nextIndex = index + 1 >= Servers.Count ? 0 : index + 1;
      var newHost = Servers[nextIndex];
      return new UriBuilder(link) { Host = newHost.Host, Port = -1 }.Uri;
    }

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на страницу.</param>
    /// <param name="imageLink">Ссылка на изображение.</param>
    /// <param name="number">Номер страницы.</param>
    /// <param name="servers">Сервера, которые могут содержать страницу.</param>
    public GroupleMangaPage(Uri uri, Uri imageLink, int number, IEnumerable<Uri> servers) : base(uri, imageLink, number)
    {
      this.Servers = servers.ToList();
      this.MaxAttempt = Math.Max(MaxAttempt, Servers.Count);
    }

    protected GroupleMangaPage() : base()
    {

    }

    #endregion
  }
}