using System;

namespace MangaReader.Services
{
  static class TypeHelper
  {
    public static Guid MangaType(this Type type)
    {
      var find = type.GetProperty("Type").GetValue(null);
      return find is Guid ? (Guid)find : Guid.Empty;
    }
  }
}
