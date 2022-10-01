﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core
{
  public abstract class BaseSiteParser : ISiteParser
  {
    protected static ConcurrentDictionary<Type, IMapper> Mappers = new ConcurrentDictionary<Type, IMapper>();
    protected readonly PluginManager pluginManager;
    protected readonly Config config;
    protected readonly IPlugin plugin;

    public abstract Task UpdateNameAndStatus(IManga manga);

    public virtual Task UpdateContentType(IManga manga)
    {
      // Content type can be not changed.
      return Task.CompletedTask;
    }

    public abstract Task UpdateContent(IManga manga);

    public abstract Task UpdatePages(Chapter chapter);

    public abstract UriParseResult ParseUri(Uri uri);

    protected static void UpdateName(IManga manga, string newName)
    {
      if (string.IsNullOrWhiteSpace(newName))
        Log.AddFormat("Не удалось получить имя манги, текущее название - '{0}'.", manga.ServerName);
      else if (newName != manga.ServerName)
        manga.ServerName = newName;
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="chapters">Главы.</param>
    protected void FillMangaChapters(IManga manga, ICollection<ChapterDto> chapters)
    {
      if (!chapters.Any())
        return;

      GetMapper().Map(chapters, manga.Chapters);
      foreach (var chapter in manga.Chapters)
      {
        chapter.Manga = manga;
        foreach (var page in chapter.Container)
        {
          page.Chapter = chapter;
        }
      }
    }

    /// <summary>
    /// Обновляем старые и добавляем новые главы и тома.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="volumes">Тома.</param>
    /// <remarks>Перенос главы из тома в том видимо не ловим.</remarks>
    protected void FillMangaVolumes(IManga manga, ICollection<VolumeDto> volumes)
    {
      if (!volumes.Any())
        return;

      GetMapper().Map(volumes, manga.Volumes);
      foreach (var volume in manga.Volumes)
      {
        volume.Manga = manga;
        foreach (var chapter in volume.Container)
        {
          chapter.Volume = volume;
          foreach (var page in chapter.Container)
          {
            page.Chapter = chapter;
          }
        }
      }
    }

    /// <summary>
    /// Обновляем страницы.
    /// </summary>
    /// <param name="manga">Манга.</param>
    /// <param name="pages">Страницы.</param>
    protected void FillMangaPages(IManga manga, ICollection<MangaPageDto> pages)
    {
      if (!pages.Any())
        return;

      GetMapper().Map(pages, manga.Pages);
      foreach (var page in manga.Pages)
      {
        page.Manga = manga;
      }
    }

    public abstract Task<IEnumerable<byte[]>> GetPreviews(IManga manga);

    public virtual IAsyncEnumerable<IManga> Search(string name)
    {
      var hosts = pluginManager.Plugins
        .Where(p => p.GetParser().GetType() == this.GetType())
        .Select(p => p.GetSettings().MainUri);
      return hosts.SelectAsync(async host => await GetMangaNodes(name, host).ConfigureAwait(false))
        .Where(nc => nc.Nodes != null)
        .SelectMany(n => n.Nodes.SelectAsync(node => GetMangaFromNode(n.Uri, n.CookieClient, node)))
        .Where(m => m != null);
    }

    protected abstract Task<(HtmlNodeCollection Nodes, Uri Uri, ISiteHttpClient CookieClient)> GetMangaNodes(string name, Uri host);

    protected abstract Task<IManga> GetMangaFromNode(Uri host, ISiteHttpClient client, HtmlNode manga);

    public abstract IMapper GetMapper();

    public BaseSiteParser(PluginManager pluginManager, Config config, IPlugin plugin)
    {
      this.pluginManager = pluginManager;
      this.config = config;
      this.plugin = plugin;
    }
  }
}
