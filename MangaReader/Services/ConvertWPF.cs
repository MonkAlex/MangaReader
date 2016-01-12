using System;

namespace MangaReader.Services
{
  public static class ConvertWPF
  {
    private static Converting dialog;

    static ConvertWPF()
    {
      Converter.ConvertStarted += ConverterOnConvertStarted;
      Converter.ConvertCompleted += ConverterOnConvertCompleted;
    }

    private static void ConverterOnConvertCompleted(object sender, EventArgs eventArgs)
    {
      dialog?.Close();
    }

    private static void ConverterOnConvertStarted(object sender, Action e)
    {
      dialog = new Converting();
      dialog.ShowDialog(e);
    }

    public static void Convert()
    {
      Converter.Convert();
    }
  }
}