using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Services
{

  class Serializer<T>
  {
    /// <summary>
    /// Загрузить из файла.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load(string path)
    {
      var type = typeof(T);
      T retVal;

      var formatter = new XmlSerializer(type);

      if (type == typeof(ObservableCollection<Mangas>))
      {
        var subTypes = ConfigStorage.Plugins.Select(p => p.MangaType).Distinct().ToArray();
        formatter = new XmlSerializer(type, subTypes);
      }

      try
      {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          retVal = (T)formatter.Deserialize(stream);
        }
      }
      catch (System.Exception)
      {
        return default(T);
      }

      return retVal;
    }
  }
}
