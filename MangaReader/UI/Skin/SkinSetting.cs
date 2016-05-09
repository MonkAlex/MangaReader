using System;

namespace MangaReader.UI.Skin
{
  public abstract class SkinSetting
  {
    public string Name { get; set; }

    public Guid Guid { get; set; }

    protected void RegisterModelAndView<TModel, TView>()
    {
      Services.ViewResolver.Instance.AddOrReplace(typeof(TModel), typeof(TView));
    }

    public abstract void Init();

    protected SkinSetting()
    {
      this.Name = "Нужно переопределить имя в конструкторе.";
    } 
  }
}