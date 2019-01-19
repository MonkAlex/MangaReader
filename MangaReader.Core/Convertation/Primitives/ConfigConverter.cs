namespace MangaReader.Core.Convertation.Primitives
{
  public abstract class ConfigConverter : BaseConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected ConfigConverter()
    {
      this.Name = "Проверка настроек...";
    }
  }
}