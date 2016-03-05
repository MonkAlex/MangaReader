using System.Collections.Generic;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public static class ContextMenuItemHelper
  {
    public static void Add(this IList<ContentMenuItem> list, BaseCommand command)
    {
      list.Add(new ContentMenuItem(command));
    }
  }
}