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
      AddOrReplace(typeof(ViewModel.Setting.SettingModel), typeof(MangaReader.SettingsForm));
      AddOrReplace(typeof(ViewModel.Setting.AppSettingModel), typeof(MangaReader.UI.AppSettingView));
      AddOrReplace(typeof(ViewModel.Setting.MangaSettingModel), typeof(MangaReader.UI.MangaSettings));
      AddOrReplace(typeof(ViewModel.Manga.MangaCardModel), typeof(MangaReader.Manga.MangaForm));

      // Пока явно забиваемся на Table.
      AddOrReplace(typeof(ViewModel.MainPageModel), typeof(MangaReader.UI.MainForm.Blazard));
//      AddOrReplace(typeof(ViewModel.MainPageModel), typeof(MangaReader.Table));
    }
  }
}