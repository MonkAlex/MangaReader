namespace MangaReader.UI.Skin
{
  public class Default : SkinSetting
  {
    public static System.Guid DefaultGuid { get { return System.Guid.Parse("D1A5A9EF-2D65-4476-8375-60CC3ECCCDE7"); } }

    public override void Init()
    {
      RegisterModelAndView<ViewModel.MainPageModel, MainForm.Table>();
    }

    public Default()
    {
      this.Name = "Стандартный";
      this.Guid = DefaultGuid;
    }
  }
}