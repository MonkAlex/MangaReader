using System;
using System.Linq;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.UI.MainForm;
using MangaReader.ViewModel;

namespace MangaReader.Services
{
  public static class LibraryWPF
  {
    /// <summary>
    /// Инициализация библиотеки - заполнение массива из кеша.
    /// </summary>
    /// <returns></returns>
    public static void Initialize(BaseForm main)
    {
      Library.SelectedManga = main.Model.View.Cast<Mangas>().FirstOrDefault();
      Library.UpdateStarted += LibraryOnUpdateStarted;
      Library.UpdateCompleted += LibraryOnUpdateCompleted;
      Library.UpdateMangaCompleted += LibraryOnUpdateMangaCompleted;
      Library.UpdatePercentChanged += LibraryOnUpdatePercentChanged;
    }

    private static void LibraryOnUpdatePercentChanged(object sender, double i)
    {
      WindowModel.Instance.Percent = i;
    }

    private static void LibraryOnUpdateMangaCompleted(object sender, Mangas mangas)
    {
      WindowModel.Instance.TaskbarIcon.ShowInTray(Strings.Library_Status_MangaUpdate + mangas.Name + " завершено.", mangas);
    }

    private static void LibraryOnUpdateCompleted(object sender, EventArgs eventArgs)
    {
      WindowModel.Instance.Percent = 0;
      WindowModel.Instance.ProgressState = ProgressState.None;
    }

    private static void LibraryOnUpdateStarted(object sender, EventArgs eventArgs)
    {
      WindowModel.Instance.Percent = 0;
      WindowModel.Instance.ProgressState = ProgressState.Normal;
    }
  }
}