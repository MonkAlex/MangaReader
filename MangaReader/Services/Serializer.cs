using System;
using System.IO;
using System.Xml.Serialization;

namespace MangaReader
{

    class Serializer<T>
    {
        /// <summary>
        /// Сохранить в файл.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void Save(string path, T data)
        {   
            var formatter = new XmlSerializer(typeof(T));

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                formatter.Serialize(stream, data);
            }
        }

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

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    retVal = (T) formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                return default(T);
            }

            return retVal;
        }
    }
}
