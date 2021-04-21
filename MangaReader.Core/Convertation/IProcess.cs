namespace MangaReader.Core.Convertation
{
  public interface IProcess
  {
    double Percent { get; set; }

    bool IsIndeterminate { get; }

    ProgressState ProgressState { get; set; }

    string Status { get; set; }

    System.Version Version { get; set; }

    ConvertState State { get; set; }

    event System.EventHandler<ConvertState> StateChanged;
  }
}
