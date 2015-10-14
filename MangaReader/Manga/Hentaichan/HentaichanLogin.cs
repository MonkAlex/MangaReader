using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using MangaReader.Account;
using MangaReader.Services;

namespace MangaReader.Manga.Hentaichan
{
  public class HentaichanLogin : Account.Login
  {
    public new static Guid Type { get { return Guid.Parse("03CEFF67-1472-438A-A90A-07B44F6FFDC4"); } }

    public virtual string UserId { get; set; }

    public virtual string PasswordHash
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.hash))
          this.hash = this.GetPasswordHash();
        return this.hash;
      }
      set { this.hash = value; }
    }

    private string hash = string.Empty;

    public virtual string GetPasswordHash()
    {
      if (string.IsNullOrWhiteSpace(this.Password))
        return string.Empty;

      using (var md5 = MD5.Create())
      {
        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(this.Password))).Replace("-", "").ToLower();
      }
    }

    /// <summary>
    /// Авторизоваться на сайте.
    /// </summary>
    public virtual void Login(CookieClient client)
    {
      if (!this.CanLogin)
        return;

      var loginData = new NameValueCollection
            {
                {"login", "submit"},
                {"login_name", this.Name},
                {"login_password", this.Password},
                {"image", "%D0%92%D1%85%D0%BE%D0%B4"}
            };

      try
      {
        client.UploadValues("http://hentaichan.ru/index.php", "POST", loginData);
        this.UserId = client.Cookie.GetCookies(new Uri(@"http:\\hentaichan.ru")).Cast<Cookie>().Single(c => c.Name == "dle_user_id").Value;
      }
      catch (Exception ex)
      {
        Log.Exception(ex);
      }
    }
  }
}