using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Orleans.Runtime;
using Orleans.TestKit.Reminders;
using TestGrains;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class ReminderTests : TestKitBase
    {
        [Fact]
        public async Task RegisterReminder()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName = "abc123";
            var due = TimeSpan.Zero;
            var period = TimeSpan.MaxValue;
            var grainId = GrainIdKeyExtensions.CreateIntegerKey(0);

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName, due, period);
            }

            // Assert
            Silo.ReminderRegistry.Mock.Verify(x => x.RegisterOrUpdateReminder(Silo.GetGrainId(grain), reminderName, due, period));
        }

        [Fact]
        public async Task UnRegisterReminder()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName = "abc123";
            var due = TimeSpan.Zero;
            var period = TimeSpan.MaxValue;

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName, due, period);
                await grain.UnregisterReminder(reminderName);
            }
           

            // Assert
             Silo.ReminderRegistry.Mock.Verify(x => x.UnregisterReminder(Silo.GetGrainId(grain), It.Is<IGrainReminder>(r => r.ReminderName == reminderName)));
        }

        [Fact]
        public async Task TriggerUnRegisterReminder()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName = "abc123";
            var due = TimeSpan.Zero;
            var period = TimeSpan.MaxValue;
           

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName, due, period);
                await grain.UnregisterReminder(reminderName);
                await Silo.FireAllReminders();
            }

            // Assert
            grain.FiredReminders.Count.Should().Be(0);
        }

        [Fact]
        public async Task TriggerAllReminders()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName1 = "abc123";
            const string reminderName2 = "123";


            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName1, TimeSpan.Zero, TimeSpan.MaxValue);
                await grain.RegisterReminder(reminderName2, TimeSpan.Zero, TimeSpan.MaxValue);
                await Silo.FireAllReminders();
            }

            // Assert
            grain.FiredReminders.Should().Contain(reminderName1);
            grain.FiredReminders.Should().Contain(reminderName2);
            grain.FiredReminders.Count.Should().Be(2);
        }

        [Fact]
        public async Task TriggerSingleReminderOnce()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName = "abc123";

         

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName, TimeSpan.Zero, TimeSpan.MaxValue);
                await Silo.FireAllReminders();
            }

            // Assert
            grain.FiredReminders.Should().Contain(reminderName);
            grain.FiredReminders.Count.Should().Be(1);
        }

        [Fact]
        public async Task TriggerSingleReminderMultiple()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            const string reminderName = "abc123";

        
            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder(reminderName, TimeSpan.Zero, TimeSpan.MaxValue);
                await Silo.FireReminder(reminderName);
                await Silo.FireReminder(reminderName);
            }
           

            // Assert
            grain.FiredReminders.Should().Contain(reminderName);
            grain.FiredReminders.Count.Should().Be(2);
        }

        [Fact]
        public async Task TriggerUnknownReminder()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);
            Func<Task> f = async () => { await Silo.FireReminder("b"); };

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.RegisterReminder("a", TimeSpan.Zero, TimeSpan.MaxValue);
            }

            // Assert
            await f.Should().ThrowAsync<Exception>();
            grain.FiredReminders.Count.Should().Be(0);
        }

        [Fact]
        public async Task UnregisterUnknownReminder()
        {
            // Arrange
            var grain = await Silo.CreateGrainAsync<HelloReminders>(0);

            // Act
            using (await Silo.GetReminderActivationContext(grain))
            {
                await grain.UnregisterReminder("a");
            }
           

            // Assert
             Silo.ReminderRegistry.Mock.Verify(v => v.GetReminder(Silo.GetGrainId(grain), "a"), Times.Once);
        }
    }
}
