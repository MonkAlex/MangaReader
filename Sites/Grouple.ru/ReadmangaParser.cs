using AutoMapper;
using AutoMapper.EquivalencyExpression;
using MangaReader.Core.Account;
using MangaReader.Core.DataTrasferObject;
using MangaReader.Core.Manga;

namespace Grouple
{
  public class ReadmangaParser : GroupleParser
  {
    public override CookieClient GetClient()
    {
      return new ReadmangaClient();
    }

    public override IMapper GetMapper()
    {
      return Mappers.GetOrAdd(typeof(ReadmangaParser), type =>
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
