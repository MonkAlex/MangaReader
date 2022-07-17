using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Tests.Entities
{
  public static class AppveyorHelper
  {
    public static bool IsRunning()
    {
      return System.Environment.GetEnvironmentVariable("APPVEYOR") != null;
    }
  }

  public class GroupleAttribute : Attribute, NUnit.Framework.Interfaces.IApplyToTest
  {
    private static IgnoreAttribute Ignore = new IgnoreAttribute("Требуется СНГ прокся");

    public void ApplyToTest(Test test)
    {
      if (AppveyorHelper.IsRunning())
      {
        Ignore.ApplyToTest(test);
      }
    }
  }

  public class ReadMangaAttribute : GroupleAttribute { }

  public class MintMangaAttribute : GroupleAttribute { }

  public class IssueAttribute : IgnoreAttribute
  {
    public IssueAttribute(ushort issue) : base($"Детали в https://github.com/MonkAlex/MangaReader/issues/{issue}")
    {
    }
  }
}
