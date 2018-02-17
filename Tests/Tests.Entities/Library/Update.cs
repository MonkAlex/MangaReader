using System;
using System.ComponentModel;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using NUnit.Framework;

namespace Tests.Entities.Library
{
  [TestFixture]
  [Parallelizable(ParallelScope.None)]
  public class Update : TestClass
  {
    [Test]
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

  }
}