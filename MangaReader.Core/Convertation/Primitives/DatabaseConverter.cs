namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class DatabaseConverter : BaseConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected DatabaseConverter()
    {
      this.Name = "Конвертация базы данных...";
    }
  }
}