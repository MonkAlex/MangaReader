namespace MangaReader.Core.Entity
{
  public interface IEntity
  {
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    void Save();

    /// <summary>
    /// Событие перед сохранением - для валидации.
    /// </summary>
    /// <param name="args">Информация о событии.</param>
    void BeforeSave(ChangeTrackerArgs args);

    /// <summary>
    ///  Загрузить свежую информацию из базы.
    /// </summary>
    void Update();

    /// <summary>
    /// Удалить из базы.
    /// </summary>
    /// <returns>False, если сущности в базе ещё не было.</returns>
    bool Delete();
  }

  public class ChangeTrackerArgs
  {
    public object[] CurrentState;
    public object[] PreviousState;
    public string[] PropertyNames;

    public ChangeTrackerArgs(object[] currentState, object[] previousState, string[] propertyNames)
    {
      this.CurrentState = currentState;
      this.PreviousState = previousState;
      this.PropertyNames = propertyNames;
    }
  }
}
