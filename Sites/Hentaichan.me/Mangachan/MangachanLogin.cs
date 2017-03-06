using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MangaReader.Core.Manga;

namespace Hentaichan.Mangachan
{
  public class MangachanLogin : BaseLogin
  {
    protected override Task<List<IManga>> DownloadBookmarks()
    {
      throw new NotImplementedException();
    }

    public MangachanLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"http://mangachan.me/");
    }
  }
}
