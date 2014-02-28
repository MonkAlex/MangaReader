using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaReader
{
    public class Settings
    {
        /// <summary>
        /// Язык манги.
        /// </summary>
        public static Languages Language { get; set; }

        /// <summary>
        /// Доступные языки.
        /// </summary>
        public enum Languages
        {
            English,
            Russian,
            Japanese
        }

        public Settings()
        {
            throw new Exception("Use methods.");
        }
    }
}
