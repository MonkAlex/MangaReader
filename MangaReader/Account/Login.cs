namespace MangaReader.Account
{
  public class Login : Entity.Entity
  {
    public virtual string Name { get; set; }

    public virtual string Password { get; set; }

    public virtual bool CanLogin { get { return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password); } }

  }
}
