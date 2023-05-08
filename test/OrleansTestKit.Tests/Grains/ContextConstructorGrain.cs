namespace OrleansTestKit.Tests.Grains;
public class ContextConstructorGrain : Grain, IGrainWithIntegerKey
{
    public ContextConstructorGrain()
    {
        var id = ((IGrainBase)this).GrainContext.GrainId;

        GrainFactory.Equals(null); // this will blow up w/ a null ref exception if runtime isn't set properly
    }
}
