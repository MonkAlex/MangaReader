using System;

namespace MangaReader.UI.Services
{
  public class ViewResolver : SimpleDictionary<Type, Type>
  {
    private static Lazy<ViewResolver> instance = new Lazy<ViewResolver>(() => new ViewResolver());

    public static ViewResolver Instance { get { return instance.Value; } }

    public void ViewInit()
    {
      AddOrReplace(typeof(ViewModel.Initialize), typeof(MangaReader.Services.Converting));
      AddOrReplace(typeof(ViewModel.DownloadUpdate), typeof(MangaReader.Services.Converting));
      AddOrReplace(typeof(ViewModel.LoginModel), typeof(MangaReader.UI.AddNewManga.Login));
      AddOrReplace(typeof(ViewModel.AddNewModel), typeof(MangaReader.UI.AddNewManga.AddNew));
      AddOrReplace(typeof(ViewModel.WindowModel), typeof(MangaReader.UI.MainWindow));
      AddOrReplace(typeof(ViewModel.VersionHistoryModel), typeof(MangaReader.Services.VersionHistoryView));
      AddOrReplace(typeof(ViewModel.SettingModel), typeof(MangaReader.SettingsForm));

      // Пока явно забиваемся на Table.
      AddOrReplace(typeof(ViewModel.MainPageModel), typeof(MangaReader.Table));
    }
  }
}