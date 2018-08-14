namespace MangaReader.Core.Entity
{
  public interface IEntity
  {
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Событие перед сохранением - для валидации.
    /// </summary>
    /// <param name="args">Информация о событии.</param>
    void BeforeSave(ChangeTrackerArgs args);
  }
}
