using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class AddNewModel : BaseViewModel
  {
    protected internal Window window { get { return this.view as Window; } }

    public List<MangaSetting> MangaSettings { get; set; }

    public ICommand Add { get; set; }

    public AddNewModel(Window view) : base(view)
    {
      this.MangaSettings = ConfigStorage.Instance.DatabaseConfig.MangaSettings;
      this.Add = new AddSelected(this);
    }
  }
}