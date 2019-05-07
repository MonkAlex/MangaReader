namespace MangaReader.Core.Entity
{
  public class ChangeTrackerArgs
  {
    private readonly object[] currentState;
    private readonly object[] previousState;
    private readonly string[] propertyNames;
    public readonly bool CanAddEntities;
    public readonly bool IsNewEntity;

    public PropertyChangeTracker<T> GetPropertyState<T>(string propertyName) where T : class
    {
      var propertyIndex = System.Array.IndexOf(propertyNames, propertyName);
      return new PropertyChangeTracker<T>(currentState[propertyIndex] as T, previousState?[propertyIndex] as T);
    }

    public void SetPropertyState(string propertyName, object value)
    {
      var propertyIndex = System.Array.IndexOf(propertyNames, propertyName);
      currentState[propertyIndex] = value;
    }

    public ChangeTrackerArgs(object[] currentState, object[] previousState, string[] propertyNames, bool canAddEntities)
    {
      this.currentState = currentState;
      this.previousState = previousState;
      this.propertyNames = propertyNames;
      this.CanAddEntities = canAddEntities;
      this.IsNewEntity = this.previousState == null;
    }
  }

  public struct PropertyChangeTracker<T> where T : class
  {
    public T Value;
    public T OldValue;
    public bool IsChanged;

    public PropertyChangeTracker(T value, T oldValue)
    {
      Value = value;
      OldValue = oldValue;
      IsChanged = !Equals(value, oldValue);
    }
  }
}
