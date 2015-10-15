using System;

namespace MangaReader.Services
{
  static class TypeHelper
  {
    public static Guid TypeProperty(this Type type)
    {
      var find = type.GetProperty("Type").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }

    public static Guid MangaProperty(this Type type)
    {
      var find = type.GetProperty("Manga").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }
  }
}
