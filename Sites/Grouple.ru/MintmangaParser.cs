﻿using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using MangaReader.Core;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace Grouple
{
  public class MintmangaParser : GroupleParser
  {
    public MintmangaParser(PluginManager pluginManager, Config config, IPlugin plugin) : base(pluginManager, config, plugin)
    {

    }

    protected override Task<ISiteHttpClient> GetClient()
    {
      return plugin.GetCookieClient(true);
    }

    public override IMapper GetMapper()
    {
      return Mappers.GetOrAdd(typeof(MintmangaParser), type =>
      {
        var config = new MapperConfiguration(cfg =>
        {
          cfg.AddCollectionMappers();
          cfg.CreateMap<VolumeDto, Volume>()
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new GroupleChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<ChapterDto, GroupleChapter>()
            .IncludeBase<ChapterDto, MangaReader.Core.Manga.Chapter>()
            .ConstructUsing(dto => new GroupleChapter(dto.Uri, dto.Name))
            .EqualityComparison((src, dest) => src.Number == dest.Number);
          cfg.CreateMap<MangaPageDto, MangaPage>()
            .EqualityComparison((src, dest) => src.ImageLink == dest.ImageLink);
        });
        return config.CreateMapper();
      });
    }
  }
}
