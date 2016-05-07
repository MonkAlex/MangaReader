using System;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.UI.MainForm;
using MangaReader.ViewModel;
using MangaReader.ViewModel.Manga;

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
#warning Blazard form
//    Library.SelectedManga = main.Model.View.Cast<MangaViewModel>().Select(vm => vm.Manga).FirstOrDefault();
      Library.UpdateStarted += LibraryOnUpdateStarted;
      Library.UpdateCompleted += LibraryOnUpdateCompleted;
      Library.UpdateMangaCompleted += LibraryOnUpdateMangaCompleted;
      Library.UpdatePercentChanged += LibraryOnUpdatePercentChanged;
      Library.PauseChanged += LibraryOnPauseChanged;
      Library.StatusChanged += (sender, s) => main.Model.LibraryStatus = s;
    }

    private static ProgressState beforePause = ProgressState.None;

    private static void LibraryOnPauseChanged(object sender, bool e)
    {
      if (e)
      {
        beforePause = WindowModel.Instance.ProgressState;
        WindowModel.Instance.ProgressState = ProgressState.Paused;
      }
      else
        WindowModel.Instance.ProgressState = beforePause;
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