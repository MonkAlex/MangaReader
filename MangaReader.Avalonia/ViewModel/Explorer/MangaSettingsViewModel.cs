using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Services;
using System.Windows.Input;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class MangaSettingsViewModel : ExplorerTabViewModel
  {
    private readonly int mangaSettingsId;

    public bool? Compress
    {
      get => compress;
      set => RaiseAndSetIfChanged(ref compress, value);
    }

    private bool? compress;

    public bool? OnlyUpdate
    {
      get => onlyUpdate;
      set => RaiseAndSetIfChanged(ref onlyUpdate, value);
    }

    private bool? onlyUpdate;

    public string Folder
    {
      get => folder;
      set => RaiseAndSetIfChanged(ref folder, value);
    }

    private string folder;

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel);
      if (!(newModel is SettingsViewModel || newModel is MangaSettingsViewModel))
      {
        foreach (var tab in ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().ToList())
          ExplorerViewModel.Instance.Tabs.Remove(tab);
      }
    }

    public ICommand Save { get; }

    public ICommand UndoChanged { get; }

    private void ReloadConfig()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = context.Get<MangaSetting>().SingleOrDefault(s => s.Id == mangaSettingsId);
        if (setting != null)
        {
          this.Compress = setting.CompressManga;
          this.OnlyUpdate = setting.OnlyUpdate;
          this.Folder = setting.Folder;
        }
      }
    }

    private void SaveConfig()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = context.Get<MangaSetting>().SingleOrDefault(s => s.Id == mangaSettingsId);
        if (setting != null)
        {
          setting.CompressManga = this.Compress == true;
          setting.OnlyUpdate = this.OnlyUpdate == true;
          setting.Folder = this.Folder;
          context.Save(setting);
        }
      }
    }

    public MangaSettingsViewModel(MangaSetting setting)
    {
      this.Name = setting.MangaName;
      this.Priority = 500;
      this.mangaSettingsId = setting.Id;

      ReloadConfig();
      this.Save = new DelegateCommand(SaveConfig);
      this.UndoChanged = new DelegateCommand(ReloadConfig);
    }
  }
}