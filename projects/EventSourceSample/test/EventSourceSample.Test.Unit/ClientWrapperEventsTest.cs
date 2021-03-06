﻿//-----------------------------------------------------------------------
// <copyright file="ClientWrapperEventsTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace EventSourceSample.Test.Unit
{
    public class ClientWrapperEventsTest : ClientWrapperTest
    {
        public ClientWrapperEventsTest()
        {
        }

        protected override ICalculatorClientAsync CreateClient(CalculatorClientStub clientStub)
        {
            return new CalculatorClientWithEvents(clientStub, ClientEventSource.Instance);
        }
    }
}
