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
    public void TestUpdate()
    {
      var library = new LibraryViewModel();
      using (var context = Repository.GetEntityContext())
      foreach (var forDelete in context.Get<IManga>())
        forDelete.Delete();
      var manga = Builder.CreateReadmanga();
      TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.DownloadedAt));
      TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.Created));
      TestUpdateFromConfig(library, ListSortDirection.Ascending, nameof(IManga.Name));
      manga.Delete();
    }

    private void TestUpdateFromConfig(LibraryViewModel library, ListSortDirection direction, string property)
    {
      ConfigStorage.Instance.ViewConfig.LibraryFilter.SortDescription = new SortDescription()
        {Direction = direction, PropertyName = property};
      var lastErrorMessage = string.Empty;

      void OnLogReceived(LogEventStruct les)
      {
        if (les.Level == "Error")
          lastErrorMessage = les.FormattedMessage;
      }

      Log.LogReceived += OnLogReceived;
      library.Update();
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

      var manga = Builder.CreateReadmanga();
      var library = new LibraryViewModel();
      library.LibraryChanged += LibraryOnLibraryChanged;

      var inProcess = false;
      Assert.AreEqual(!inProcess, library.IsAvaible);
      Assert.AreEqual(false, library.IsPaused);
      Assert.AreEqual(inProcess, library.InProcess);

      var task = library.ThreadAction(() => library.Update(new List<int>(){manga.Id}, mangas => mangas.OrderBy(m => m.DownloadedAt)));

      Assert.AreEqual(task.IsCompleted, library.IsAvaible);
      Assert.AreEqual(false, library.IsPaused);
      Assert.AreEqual(!task.IsCompleted, library.InProcess);

      library.IsPaused = true;

      Assert.AreEqual(task.IsCompleted, library.IsAvaible);
      Assert.AreEqual(true, library.IsPaused);
      Assert.AreEqual(!task.IsCompleted, library.InProcess);

      library.IsPaused = false;

      await task;

      library.Remove(manga);
      library.LibraryChanged -= LibraryOnLibraryChanged;
      Assert.AreEqual(0, manga.Id);

      Assert.Contains(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateStarted), events);
      Assert.Contains(new LibraryViewModelArgs(null, null, MangaOperation.None, LibraryOperation.UpdateCompleted), events);
      Assert.Contains(new LibraryViewModelArgs(null, manga, MangaOperation.Deleted, LibraryOperation.UpdateMangaChanged), events);
    }

  }
}