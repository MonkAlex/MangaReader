using NUnit.Framework;

namespace Tests
{
  [Parallelizable(ParallelScope.All)]
  public class TestClass
  {
    [SetUp]
    protected void SetUp()
    {
      Environment.SetUp(true);
    }

    [TearDown]
    protected void Clean()
    {
    }
  }
}