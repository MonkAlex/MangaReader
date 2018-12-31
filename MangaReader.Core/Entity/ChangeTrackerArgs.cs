using System.Linq;

namespace MangaReader.Core.Entity
{
  public class ChangeTrackerArgs
  {
    public object[] CurrentState;
    public object[] PreviousState;
    public string[] PropertyNames;
    public bool CanAddEntities;
    public bool IsNewEntity;

    public PropertyChangeTracker<T> GetPropertyState<T>(string propertyName) where T : class
    {
      var propertyIndex = PropertyNames.ToList().IndexOf(propertyName);
      return new PropertyChangeTracker<T>(CurrentState[propertyIndex] as T, PreviousState?[propertyIndex] as T);
    }

    public ChangeTrackerArgs(object[] currentState, object[] previousState, string[] propertyNames, bool canAddEntities)
    {
      this.CurrentState = currentState;
      this.PreviousState = previousState;
      this.PropertyNames = propertyNames;
      this.CanAddEntities = canAddEntities;
      this.IsNewEntity = PreviousState == null;
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
      IsChanged = Value != OldValue;
    }
  }
}