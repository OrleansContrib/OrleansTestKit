namespace TestInterfaces;

public interface IUnknownGrain : IGrainWithStringKey
{
    //this grain's identity is not known when it is resolved
    //for instance, if GrainA needs to create another NEW IUnknownGrain it would generate a new id and ask the GrainFactory for a reference
    //in this case a test of GrainA would not know the id in advance and would not be able to set up a probe using the id.

    Task<string> WhatsMyId();
}
