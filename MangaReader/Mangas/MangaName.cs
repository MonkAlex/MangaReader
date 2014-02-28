using System;

namespace MangaReader.Mangas
{
    public class MangaName
    {
        private string english;
        private string russian;
        private string japanese;

        /// <summary>
        /// Английское название манги.
        /// </summary>
        public string English
        {
            get 
            {
                return !string.IsNullOrEmpty(this.english) ? english : Japanese;
            }
            set 
            {
                english = value;
            }
        }

        /// <summary>
        /// Русское название манги.
        /// </summary>
        public string Russian
        {
            get 
            {
                return !string.IsNullOrEmpty(this.russian) ? russian : Japanese;
            }
            set
            {
                russian = value;
            }
        }

        /// <summary>
        /// Японское название манги.
        /// </summary>
        public string Japanese
        {
            get
            {
                if (!string.IsNullOrEmpty(this.japanese))
                    return japanese;
                else if (!string.IsNullOrEmpty(this.english))
                    return english;
                else if (!string.IsNullOrEmpty(this.russian))
                    return russian;
                else
                    return string.Empty;
            }
            set
            {
                japanese = value;
            }
        }

        /// <summary>
        /// Проверка наличия названия на любом языке.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.Japanese);
            }
        }

        /// <summary>
        /// Определение строкового значения по настройкам.
        /// </summary>
        /// <returns>Название манги.</returns>
        public override string ToString()
        {
            if (Settings.Language == Settings.Languages.English)
                return English;
            if (Settings.Language == Settings.Languages.Russian)
                return Russian;
            return Japanese;
        }

        /// <summary>
        /// Имя манги.
        /// </summary>
        public MangaName() { }

        /// <summary>
        /// Имя манги.
        /// </summary>
        /// <param name="name">Имя на любом языке. Будет записано в японский.</param>
        public MangaName(string name)
        {
            this.Japanese = name;
        }

        /// <summary>
        /// Имя манги.
        /// </summary>
        /// <param name="english">Английское название.</param>
        /// <param name="russian">Русское название.</param>
        /// <param name="japanese">Японское название.</param>
        public MangaName(string english, string russian, string japanese)
        {
            this.English = english;
            this.Russian = russian;
            this.Japanese = japanese;
        }
    }
}
