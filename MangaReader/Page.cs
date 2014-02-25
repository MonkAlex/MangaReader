using System;
using System.Text;

namespace MangaReader
{
    public static class Page
    {
        public static string GetPage(string url)
        {

                try
                {
                    var webClient = new System.Net.WebClient { Encoding = Encoding.UTF8 };
                    return webClient.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Log.Add(ex.Message, Log.Level.Information);
                }
                return string.Empty;
        }
    }
}
