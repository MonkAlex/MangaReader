using System.Collections.Generic;
using System.Linq;
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

    public List<LoginModel> Logins { get; set; }

    public ICommand Add { get; set; }

    public AddNewModel(Window view) : base(view)
    {
      this.Logins = ConfigStorage.Instance.DatabaseConfig.MangaSettings.Select(s => new LoginModel(view, s)).ToList();
      this.Add = new AddSelected(this);
    }
  }
}