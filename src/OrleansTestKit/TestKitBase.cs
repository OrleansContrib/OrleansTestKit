namespace Orleans.TestKit
{
    /// <summary>
    /// Base class used for all grain based unit tests. The <see cref="TestKitBase"/>
    /// provides tools to mock up the orleans and create the grain under test
    /// </summary>
    public abstract class TestKitBase
    {
        protected readonly TestKitSilo Silo = new TestKitSilo();
    }
}
