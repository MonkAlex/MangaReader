using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;

namespace MangaReader.Services
{
  class ImageFile
  {
    internal byte[] Body { get; set; }

    internal string Hash
    {
      get
      {
        if (this.Exist && string.IsNullOrWhiteSpace(this.hash))
          this.hash = GetHash(this.Body);
        return this.hash;
      }
    }

    private string hash = string.Empty;

    internal bool Exist { get { return this.Body != null; } }

    internal string Extension 
    {
      get
      {
        if (this.Exist && string.IsNullOrWhiteSpace(this.extension))
        {
          var created = Image.FromStream(new MemoryStream(this.Body));
          this.extension = new ImageFormatConverter().ConvertToString(created.RawFormat).ToLower();
        }
        return this.extension;
      }
    }
    private string extension = string.Empty;
    
    static string GetHash(byte[] body)
    {
      // TODO: возможно стоит переехать на sha2? Хотя, главное - скорость.
      using (var md5 = MD5.Create())
      {
        return BitConverter.ToString(md5.ComputeHash(md5.ComputeHash(body))).Replace("-", "");
      }
    }
  }
}
