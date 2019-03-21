using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Core;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel
{
  public class AddFromUri : SettingViewModel
  {
    private string inputText;
    private string hint;
    private List<ISiteParser> parsers;

    public ICommand Add { get; set; }

    public string InputText
    {
      get { return inputText; }
      set
      {
        inputText = value;
        InputChanged(value);
        OnPropertyChanged();
      }
    }

    public string Hint
    {
      get { return hint; }
      set
      {
        hint = value;
        OnPropertyChanged();
      }
    }

    public override Task Save()
    {
#warning What the hell with this base class? Only for header property?
      throw new NotImplementedException();
    }

    private void InputChanged(string newValue)
    {
      Hint = string.Empty;
      Uri uri;
      if (!Uri.TryCreate(newValue, UriKind.Absolute, out uri))
        return;

      if (parsers == null)
        parsers = ConfigStorage.Plugins.Select(p => p.GetParser()).ToList();

#warning Запрос к базе на каждый введенный символ.
      using (Repository.GetEntityContext("Load settings for parsing uri"))
        foreach (var parser in parsers)
        {
          var parseResult = parser.ParseUri(uri);
          if (!parseResult.CanBeParsed)
            continue;

          switch (parseResult.Kind)
          {
            case UriParseKind.Manga:
              Hint = "Манга будет скачана c начала";
              break;
            case UriParseKind.Volume:
              Hint = "Манга будет скачана только с указанного тома";
              break;
            case UriParseKind.Chapter:
              Hint = "Манга будет скачана только с указанной главы";
              break;
            case UriParseKind.Page:
              Hint = "Манга будет скачана только с указанной страницы";
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
          break;
        }
    }

    public AddFromUri(AddNewModel parent)
    {
      this.Add = new AddSelected(this, parent);
      this.Header = "Новая...";
    }
  }
}