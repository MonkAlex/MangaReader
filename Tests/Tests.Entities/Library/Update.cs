using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NUnit.Framework;
using SortDescription = MangaReader.Core.Services.Config.SortDescription;

namespace Tests.Entities.Library
{
  [TestFixture]
  public class Update : TestClass
  {
    [Test]
    [Parallelizable(ParallelScope.None)]
    public async Task TestUpdate()
    {
      var library = new LibraryViewModel();
      using (var context = Repository.GetEntityContext())
        foreach (var forDelete in await context.Get<IManga>().ToListAsync().ConfigureAwait(false))
          await context.Delete(forDelete).ConfigureAwait(false);
      var manga = await Builder.CreateReadmanga().ConfigureAwait(false);
      await TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.DownloadedAt)).ConfigureAwait(false);
      await TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.Created)).ConfigureAwait(false);
      await TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.Name)).ConfigureAwait(false);
      using (var context = Repository.GetEntityContext())
        await context.Delete(manga).ConfigureAwait(false);
    }

    private async Task TestUpdateFromConfig(LibraryViewModel library, ListSortDirection direction, string property)
    {
      ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription = new SortDescription()
      { Direction = direction, PropertyName = property };
      var lastErrorMessage = string.Empty;

      void OnLogReceived(LogEventStruct les)
      {
        if (les.Level == "Error")
          lastErrorMessage = les.FormattedMessage;
      }

      Log.LogReceived += OnLogReceived;
      await library.Update().ConfigureAwait(false);
      Log.LogReceived -= OnLogReceived;
      if (!string.IsNullOrWhiteSpace(lastErrorMessage))
        Assert.Fail(lastErrorMessage);
    }

    [Test]
    public async Task UpdateMangaWithPause()
    {
      var events = new List<LibraryViewModelArgs>();
      void LibraryOnLibraryChanged(object sender, LibraryViewModelArgs e)
      {
        events.Add(e);
      }

      var library = new LibraryViewModel();
      library.LibraryChanged += LibraryOnLibraryChanged;
      var (_, manga) = await library.Add(new Uri(MangaInfos.Acomics.SuperScienceFriends.Uri)).ConfigureAwait(false);

      var inProcess = false;
      Assert.AreEqual(!inProcess, library.IsAvaible);
      Assert.AreEqual(false, library.IsPaused);
      Assert.AreEqual(inProcess, library.InProcess);

      var task = library.ThreadAction(library.Update(new List<int>() { manga.Id }, mangas => mangas.OrderBy(m => m.DownloadedAt)));

      Assert.AreEqual(task.IsCompleted, library.IsAvaible);
      Assert.AreEqual(false, library.IsPaused);
      Assert.AreEqual(!task.IsCompleted, library.InProcess);

      library.IsPaused = true;

      Assert.AreEqual(task.IsCompleted, library.IsAvaible);
      Assert.AreEqual(true, library.IsPaused);
      Assert.AreEqual(!task.IsCompleted, library.InProcess);

      var (added, addedManga) = await library.Add(new Uri(MangaInfos.Mangachan.Rain.Uri)).ConfigureAwait(false);
      Assert.IsTrue(added);

      library.IsPaused = false;

      await task.ConfigureAwait(false);

      await library.Remove(manga).ConfigureAwait(false);
      await library.Remove(addedManga).ConfigureAwait(false);
      library.LibraryChanged -= LibraryOnLibraryChanged;
      Assert.AreEqual(0, manga.Id);

      Assert.Contains(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateStarted), events);
      Assert.Contains(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateCompleted), events);

      Assert.Contains(new LibraryViewModelArgs(null, manga, MangaOperation.Added, LibraryOperation.UpdateMangaChanged), events);
      Assert.Contains(new LibraryViewModelArgs(null, manga, MangaOperation.Deleted, LibraryOperation.UpdateMangaChanged), events);

      Assert.Contains(new LibraryViewModelArgs(null, addedManga, MangaOperation.Added, LibraryOperation.UpdateMangaChanged), events);
      Assert.Contains(new LibraryViewModelArgs(null, addedManga, MangaOperation.Deleted, LibraryOperation.UpdateMangaChanged), events);
    }

    [Test]
    public async Task UpdateManyMangas()
    {
      var library = new LibraryViewModel();
      var ids = Enumerable.Range(40_000, 2500).ToList();

      var lastErrorMessage = string.Empty;
      void OnLogReceived(LogEventStruct les)
      {
        if (les.Level == "Error")
          lastErrorMessage = les.FormattedMessage;
      }

      Log.LogReceived += OnLogReceived;
      await library.Update(ids).ConfigureAwait(false);
      Log.LogReceived -= OnLogReceived;
      if (!string.IsNullOrWhiteSpace(lastErrorMessage))
        Assert.Fail(lastErrorMessage);
    }

  }
}
