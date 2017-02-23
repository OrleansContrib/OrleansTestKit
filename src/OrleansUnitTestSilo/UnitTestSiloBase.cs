namespace OrleansNonSiloTesting
{
    /// <summary>
    /// Base class used for all grain based unit tests. The <see cref="UnitTestSiloBase"/>
    /// provides tools to mock up the orleans and create the grain under test
    /// </summary>
    public abstract class UnitTestSiloBase
    {
        protected readonly UnitTestSilo Silo = new UnitTestSilo();
    }
}
