using System;

namespace MangaReader.Account
{
  public class Login : Entity.Entity
  {
    public static Guid Type { get { return Guid.Parse("EC4D4CDE-EF54-4B67-AF48-1B7909709D5C"); } }

    public virtual string Name { get; set; }

    public virtual string Password { get; set; }

    public virtual bool CanLogin { get { return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password); } }

  }
}
