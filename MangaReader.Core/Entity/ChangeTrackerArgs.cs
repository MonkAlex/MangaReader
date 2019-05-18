using System;

namespace MangaReader.Core.Entity
{
  public class ChangeTrackerArgs
  {
    private readonly object[] currentState;
    private readonly object[] previousState;
    private readonly string[] propertyNames;
    public readonly bool CanAddEntities;
    public readonly bool IsNewEntity;

    /// <summary>
    /// Get property changes from session.
    /// </summary>
    /// <typeparam name="T">Type of property</typeparam>
    /// <param name="propertyName">Property name. Use nameof() for support refactoring.</param>
    /// <returns>Old-new value.</returns>
    /// <remarks>For struct use nullable type for safe invoke. Or use it only for changes, not for first creating.</remarks>
    public PropertyChangeTracker<T> GetPropertyState<T>(string propertyName)
    {
      var propertyIndex = System.Array.IndexOf(propertyNames, propertyName);
      return new PropertyChangeTracker<T>((T)currentState[propertyIndex], (T)previousState?[propertyIndex]);
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

  public struct PropertyChangeTracker<T>
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
