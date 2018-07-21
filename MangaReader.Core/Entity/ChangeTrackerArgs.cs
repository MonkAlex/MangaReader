namespace MangaReader.Core.Entity
{
  public class ChangeTrackerArgs
  {
    public object[] CurrentState;
    public object[] PreviousState;
    public string[] PropertyNames;
    public bool CanAddEntities;

    public ChangeTrackerArgs(object[] currentState, object[] previousState, string[] propertyNames, bool canAddEntities)
    {
      this.CurrentState = currentState;
      this.PreviousState = previousState;
      this.PropertyNames = propertyNames;
      this.CanAddEntities = canAddEntities;
    }
  }
}