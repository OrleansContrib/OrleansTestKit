using System.Reflection;
using FluentAssertions;
using Moq;
using Orleans.Runtime;
using Orleans.Storage;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests;

public class StorageFacetTests : TestKitBase
{
    private static readonly Guid GrainId = Guid.Parse("f267aaeb-38dd-4ff4-b7c8-20061077ab10");

    [Fact]
    public async Task GetColor_WithDefaultState_ReturnsUnknown()
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        var color = await grain.GetColor();

        // Assert
        color.Should().Be(Color.Unknown);
    }

    [Fact]
    public async Task GetColor_WithState_ReturnsColor()
    {
        // Arrange
        var state = new ColorGrainState
        {
            Color = Color.Red,
            Id = GrainId,
        };

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        var color = await grain.GetColor();

        // Assert
        color.Should().Be(Color.Red);
    }

    [Fact]
    public async Task OnActivateAsync_WithDefaultState_DoesNotMutateState()
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        // Act
        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Assert
        state.Color.Should().Be(Color.Unknown);
        state.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task ResetColor_WithDefaultState_DoesNotClearState()
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.ClearStateAsync());

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        await grain.ResetColor();

        // Assert
        state.Color.Should().Be(Color.Unknown);
        state.Id.Should().Be(Guid.Empty);

        // Note that the following assert ties this test to the _implementation_ details. Generally, one should try to
        // avoid tying the test to the implementation details. It can lead to more brittle tests. However, one may
        // choose to accept this as a trade-off when the implementation detail represents an important behavior.
        mockState.Verify(o => o.ClearStateAsync(), Times.Never);
    }

    [Fact]
    public async Task ResetColor_WithState_ClearsState()
    {
        // Arrange
        var state = new ColorGrainState
        {
            Color = Color.Red,
            Id = GrainId,
        };

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.ClearStateAsync());

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        await grain.ResetColor();

        // Assert
        mockState.Verify(o => o.ClearStateAsync(), Times.Once);
    }

    [Fact]
    public async Task SetColor_WithDefaultState_SetsState()
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.WriteStateAsync());

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        await grain.SetColor(Color.Blue);

        // Assert
        state.Color.Should().Be(Color.Blue);
        state.Id.Should().Be(GrainId);
    }

    [Theory]
    [InlineData(Color.Unknown)]
    [InlineData((Color)int.MaxValue)]
    public async Task SetColor_WithInvalidColor_ThrowsArgumentException(Color color)
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.WriteStateAsync());

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);
        Action action = () => grain.SetColor(color);

        // Act+Assert
        action.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("color");
    }

    [Fact]
    public async Task SetColor_WithMutatedState_ThrowsInconsistentStateException()
    {
        // Arrange
        var state = new ColorGrainState();

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.WriteStateAsync()).Throws<InconsistentStateException>();

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);
        Action action = () => grain.SetColor(Color.Green);

        // Act+Assert
        action.Should().Throw<InconsistentStateException>();
    }

    [Fact]
    public async Task SetColor_WithState_SetsState()
    {
        // Arrange
        var state = new ColorGrainState
        {
            Color = Color.Red,
            Id = GrainId,
        };

        var mockState = new Mock<IPersistentState<ColorGrainState>>();
        mockState.SetupGet(o => o.State).Returns(state);
        mockState.Setup(o => o.ClearStateAsync());

        var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
        mockMapper.Setup(o => o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>())).Returns(context => mockState.Object);

        Silo.AddService(mockMapper.Object);

        var grain = await Silo.CreateGrainAsync<ColorGrain>(GrainId);

        // Act
        await grain.SetColor(Color.Blue);

        // Assert
        state.Color.Should().Be(Color.Blue);
        state.Id.Should().Be(GrainId);
    }
}
