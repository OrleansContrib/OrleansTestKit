﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Orleans.TestKit;
using TestGrains;
using TestInterfaces;
using Xunit;

namespace Orleans.TestKit.Tests
{
    public class StrictGrainProbeTests : TestKitBase
    {
        public StrictGrainProbeTests()
        {
            //Enables strict grain checking for all tests
            Silo.Options.StrictGrainProbes = true;
        }

        [Fact]
        public async Task SetupProbe()
        {
            var grain = await Silo.CreateGrainAsync<PingGrain>(1);

            var pong = Silo.AddProbe<IPong>(22);

            await grain.Ping();

            pong.Verify(p => p.Pong(), Times.Once);
        }

        [Fact]
        public async Task MissingProbe()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();
        }

        [Fact]
        public async Task InvalidProbe()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            //This uses the wrong id for the IPong since this is hard coded within PingGrain
            var pong = Silo.AddProbe<IPong>(0);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();

            pong.Verify(p => p.Pong(), Times.Never);
        }

        [Fact]
        public async Task InvalidProbeType()
        {
            IPing grain = await Silo.CreateGrainAsync<PingGrain>(1);

            //This uses the correct id, but the wrong grain type
            var pong = Silo.AddProbe<IPong2>(22);

            grain.Invoking(p => p.Ping()).ShouldThrowExactly<Exception>();

            pong.Verify(p => p.Pong2(), Times.Never);
        }
    }
}
