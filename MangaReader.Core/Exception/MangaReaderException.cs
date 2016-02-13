namespace MangaReader.Core.Exception
{
  public class MangaReaderException : System.Exception
  {
    private readonly string message;

    public override string Message
    {
      get { return FormatMessage(); }
    }

    public virtual string FormatMessage()
    {
      return message;
    }

    public MangaReaderException() : this(string.Empty)
    {
    }

    public MangaReaderException(string message) : this(message, null)
    {
    }

    public MangaReaderException(string message, System.Exception inner) : base(message, inner)
    {
      this.message = message;
    }
  }
}