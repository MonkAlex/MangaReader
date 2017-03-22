using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class Initialize : ProcessModel
  {
    public override void Load()
    {
      base.Load();
      Core.Client.Init();
    }

    public override void Show()
    {
      base.Show();
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.ContentRendered += (sender, args) => Task.Run(() =>
        {
          try
          {
            Core.Client.Start(this);

            SaveSettingsCommand.ValidateMangaPaths();
          }
          catch (Exception e)
          {
            Log.Exception(e, "Инициализация не закончилась.");
          }
        });
        window.ShowDialog();
      }
    }

  }
}