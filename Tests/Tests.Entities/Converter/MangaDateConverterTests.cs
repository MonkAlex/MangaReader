using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Mangas;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using NUnit.Framework;

namespace Tests.Entities.Converter
{
  [TestFixture, Parallelizable(ParallelScope.None)]
  public class MangaDateConverterTests : TestClass
  {
    [Test]
    public async Task MangaCreatedConverter([Values]bool hasHistory, [Values]bool hasFolder, [Values]bool hasAnotherManga)
    {
      IManga manga;
      using (var context = Repository.GetEntityContext())
      {
        var forDelete = await context.Get<IManga>().ToListAsync().ConfigureAwait(false);
        foreach (var mangaForDelete in forDelete)
        {
          await context.Delete(mangaForDelete).ConfigureAwait(false);
        }

        if (hasAnotherManga)
          await Builder.CreateAcomics().ConfigureAwait(false);

        manga = await Builder.CreateReadmanga().ConfigureAwait(false);
        manga.Created = null;
        await context.Save(manga).ConfigureAwait(false);

        if (hasHistory)
          await Builder.CreateMangaHistory(manga).ConfigureAwait(false);
        if (hasFolder)
          Directory.CreateDirectory(manga.GetAbsoluteFolderPath());
      }

      var converter = new CreatedWrapper();
      await converter.Convert(new ReportProcess()).ConfigureAwait(false);

      using (var context = Repository.GetEntityContext())
      {
        manga = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Id == manga.Id).ConfigureAwait(false);

        Assert.IsNotNull(manga.Created);
        if (hasHistory)
          Assert.AreEqual(manga.Histories.Select(h => h.Date).FirstOrDefault(), manga.Created);
        else if (hasFolder)
          Assert.AreEqual(new DirectoryInfo(manga.GetAbsoluteFolderPath()).CreationTime, manga.Created);
        else if (hasAnotherManga)
          Assert.AreEqual(DateTime.Today.AddDays(-1).AddSeconds(1), manga.Created);
        else
          Assert.AreEqual(DateTime.Today.AddDays(-1), manga.Created);

        var forDelete = await context.Get<IManga>().ToListAsync().ConfigureAwait(false);
        foreach (var mangaForDelete in forDelete)
        {
          await context.Delete(mangaForDelete).ConfigureAwait(false);
        }
      }
    }

    [Test]
    public async Task MangaDownloadedAtConverter([Values] bool hasHistory)
    {
      IManga manga;
      using (var context = Repository.GetEntityContext())
      {
        manga = await Builder.CreateReadmanga().ConfigureAwait(false);
        manga.DownloadedAt = null;
        await context.Save(manga).ConfigureAwait(false);

        if (hasHistory)
          await Builder.CreateMangaHistory(manga).ConfigureAwait(false);
      }

      var converter = new DownloadedAtWrapper();
      await converter.Convert(new ReportProcess()).ConfigureAwait(false);

      using (var context = Repository.GetEntityContext())
      {
        manga = await context.Get<IManga>().FirstOrDefaultAsync(m => m.Id == manga.Id).ConfigureAwait(false);

        if (hasHistory)
          Assert.AreEqual(manga.Histories.Select(h => h.Date).FirstOrDefault(), manga.DownloadedAt);
        else
          Assert.IsNull(manga.DownloadedAt);

        var forDelete = await context.Get<IManga>().ToListAsync().ConfigureAwait(false);
        foreach (var mangaForDelete in forDelete)
        {
          await context.Delete(mangaForDelete).ConfigureAwait(false);
        }
      }
    }

    private class CreatedWrapper : From44To45Created
    {
      protected override bool ProtectedCanConvert(IProcess process)
      {
        return true;
      }
    }

    private class DownloadedAtWrapper : From44To45DownloadedAt
    {
      protected override bool ProtectedCanConvert(IProcess process)
      {
        return true;
      }
    }
  }
}
