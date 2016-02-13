using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.UI;

namespace MangaReader.ViewModel.Commands
{
  public class AddNewMangaCommand : BaseCommand
  {
    public override string Name { get { return Strings.Library_Action_Add; } }

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      var db = new Input { Owner = WindowHelper.Owner };
      if (db.ShowDialog() != true)
        return;
      try
      {
        if (!string.IsNullOrWhiteSpace(db.Result.Text))
          Library.Add(db.Result.Text);
        var dialogMangas = db.BookmarksTabs.Items.SourceCollection.Cast<TabItem>()
            .Select(t => t.Content)
            .Cast<Login>()
            .SelectMany(l => l.Bookmarks.SelectedItems.Cast<Mangas>())
            .Distinct()
            .ToList();
        foreach (var manga in dialogMangas)
          Library.Add(manga.Uri);
      }
      catch (Exception ex)
      {
        Log.Exception(ex, "Ошибка добавления манги.");
        MessageBox.Show(ex.Message);
      }
    }
  }
}