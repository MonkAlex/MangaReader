namespace MangaReader.Core.Entity
{
  public interface IEntity
  {
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Сохранить в базу.
    /// </summary>
    void Save();

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
}
