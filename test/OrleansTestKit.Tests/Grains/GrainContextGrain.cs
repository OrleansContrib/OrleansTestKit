using Orleans.Runtime;

namespace Orleans.TestKit.Tests.Grains;
internal class GrainContextGrain : Grain, IGrainWithGuidKey
{
    public IGrainContext Context { get; set; }

    public GrainContextGrain(IGrainContext context)
    {
        Context = context;
    }
}

internal sealed class GrainContextGrain2 : GrainContextGrain
{
    public GrainContextGrain2(IGrainContext context) : base(context) { }
}

internal sealed class GrainContextIntegerGrain : Grain, IGrainWithIntegerKey
{ }

internal sealed class GrainContextStringGrain : Grain, IGrainWithStringKey
{ }

internal sealed class GrainContextGuidGrain : Grain, IGrainWithGuidKey
{ }

internal sealed class GrainContextGuidCompoundGrain : Grain, IGrainWithGuidCompoundKey
{ }

internal sealed class GrainContextIntegerCompoundGrain : Grain, IGrainWithIntegerCompoundKey
{ }
