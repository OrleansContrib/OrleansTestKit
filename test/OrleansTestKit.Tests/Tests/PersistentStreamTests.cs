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
    public class PersistentStreamTests : TestKitBase
    {

        private readonly PersistentListenerState _state;
        private readonly Mock<IPersistentState<PersistentListenerState>> _persistentState;
        private readonly TestStream<ChatMessage> _stream;

        public PersistentStreamTests()
        {
            _state = new PersistentListenerState();

            _persistentState = new Mock<IPersistentState<PersistentListenerState>>();
            _persistentState.SetupGet(o => o.State).Returns(_state);

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
            await Silo.CreateGrainAsync<PersistentListener>(1);


            //Assert
            Assert.Equal(1, _stream.Subscribed);
            Assert.NotNull(_state.ChatMessageStreamSubscriptionHandle);
            _persistentState.Verify(x => x.WriteStateAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenDeactivatedGrainThatHasSetupAHandler_WhenActivating_ThenResumeHandlerAndHandleNewMessages()
        {
            //Arrange
            //Stream Setup
            var grain = await Silo.CreateGrainAsync<PersistentListener>(1);

            //Check to see there is a handler registered and a stream subscriber
            var initialHandlers = await _stream.GetAllSubscriptionHandles();
            Assert.Equal(1, initialHandlers.Count);
            Assert.Equal(1, _stream.Subscribed);

            Assert.NotNull(_state.ChatMessageStreamSubscriptionHandle);

            //Stream Works
            await _stream.OnNextAsync(new ChatMessage("hi!"));
            var initialMessageCount = await grain.ReceivedCount();
            Assert.Equal(1, initialMessageCount);

            //Deactivate grain
            await Silo.DeactivateAsync(grain);

            //Check to see there is a handler registered
            var handlersAfterDeactivating = await _stream.GetAllSubscriptionHandles();
            Assert.Equal(1, handlersAfterDeactivating.Count);
            Assert.Equal(_state.ChatMessageStreamSubscriptionHandle, handlersAfterDeactivating.First());



            //Act
            //Reactivate the grain
            await Silo.ReactivateAsync(grain);
            _persistentState.Invocations.Clear();

            //Send another message
            await _stream.OnNextAsync(new ChatMessage("hi to you too!"));

            //State updated
            var finalMessageCount = await grain.ReceivedCount();
            Assert.Equal(2, finalMessageCount);
            _persistentState.Verify(x => x.WriteStateAsync(), Times.Once);

            //Still only one subscriber
            Assert.Equal(1, _stream.Subscribed);
        }

        [Fact]
        public async Task GivenEmptyHandlerInState_WhenGrainActivates_ThenResumeHandler()
        {
            //Arrange
            var onResumeCalled = false;
            var onAttachingObserver = new Action<IAsyncObserver<ChatMessage>>(obs => onResumeCalled = true);

            _state.ChatMessageStreamSubscriptionHandle =
                await _stream.AddEmptyStreamHandler(onAttachingObserver);

            //Check to see there is a handler registered
            var handles = await _stream.GetAllSubscriptionHandles();
            Assert.Equal(1, handles.Count);

            //Act
            var grain = await Silo.CreateGrainAsync<PersistentListener>(1);

            //Assert
            Assert.True(onResumeCalled);

            //Act
            await _stream.OnNextAsync(new ChatMessage("let there be life!"));

            //Assert
            var receivedMessages = await grain.ReceivedCount();
            Assert.Equal(1, receivedMessages);

        }
    }
}
