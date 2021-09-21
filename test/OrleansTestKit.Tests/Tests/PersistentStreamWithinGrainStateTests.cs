using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestKit.Streams;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class PersistentStreamWithinGrainStateTests : TestKitBase
    {

        private readonly PersistentListenerStateWithHandle _stateWithHandle;
        private readonly Mock<IPersistentState<PersistentListenerStateWithHandle>> _persistentState;
        private readonly TestStream<ChatMessage> _stream;

        public PersistentStreamWithinGrainStateTests()
        {
            _stateWithHandle = new PersistentListenerStateWithHandle();

            _persistentState = new Mock<IPersistentState<PersistentListenerStateWithHandle>>();
            _persistentState.SetupGet(o => o.State).Returns(_stateWithHandle);

            var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
            mockMapper.Setup(o =>
                    o.GetFactory(It.IsAny<ParameterInfo>(), It.IsAny<PersistentStateAttribute>()))
                .Returns(context => _persistentState.Object);


            Silo.AddService(mockMapper.Object);

            _stream = Silo.AddStreamProbe<ChatMessage>(Guid.Empty, null, "Default");
        }


        [Fact]
        public async Task WhenActivated_StoreStreamHandlerInState()
        {
            //Arrange + Act
            await Silo.CreateGrainAsync<PersistentListenerWithHandleInState>(1);

            //Assert
            Assert.Equal(1, _stream.Subscribed);
            Assert.NotNull(_stateWithHandle.ChatMessageStreamSubscriptionHandle);
            _persistentState.Verify(x => x.WriteStateAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenEmptyHandlerInState_WhenGrainActivates_ThenResumeHandler()
        {
            //Arrange
            var onResumeCalled = false;
            var onAttachingObserver = new Action<IAsyncObserver<ChatMessage>>(obs => onResumeCalled = true);

            _stateWithHandle.ChatMessageStreamSubscriptionHandle =
                await _stream.AddEmptyStreamHandler(onAttachingObserver);

            //Check to see there is a handler registered
            var handles = await _stream.GetAllSubscriptionHandles();
            Assert.Equal(1, handles.Count);
            Assert.False(onResumeCalled);

            //Act
            var grain = await Silo.CreateGrainAsync<PersistentListenerWithHandleInState>(1);

            //Assert
            Assert.True(onResumeCalled);

            //Act
            await _stream.OnNextAsync(new ChatMessage("here's a message"));

            //Assert
            var receivedMessages = await grain.ReceivedCount();
            Assert.Equal(1, receivedMessages);
        }

        
    }
}
