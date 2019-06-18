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
    System.Threading.Tasks.Task BeforeSave(ChangeTrackerArgs args);

    /// <summary>
    /// Событие перед удалением - для валидации.
    /// </summary>
    /// <param name="args">Информация о событии.</param>
    System.Threading.Tasks.Task BeforeDelete(ChangeTrackerArgs args);
  }
}
