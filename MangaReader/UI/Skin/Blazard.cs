namespace MangaReader.UI.Skin
{
  public class Blazard : SkinSetting
  {
    public override void Init()
    {
      RegisterModelAndView<ViewModel.MainPageModel, MainForm.Blazard>();
      RegisterModelAndView<ViewModel.Manga.MangaViewModel, Manga.BlazardManga>();
    }

    public Blazard()
    {
      this.Name = "Blazard";
      this.Guid = System.Guid.Parse("6D4F6E00-950B-4FC2-A114-2A3A60BF9648");
    }
  }
}