namespace MangaReader.Account
{
  public class Login : Entity.Entity
  {
    public virtual string Name { get; set; }

    public virtual string Password { get; set; }
  }
}
