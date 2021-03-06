﻿//-----------------------------------------------------------------------
// <copyright file="EventWindowCollector.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace TraceAnalysisSample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventWindowCollector : IEventCollector
    {
        private EventWindow window;

        public EventWindowCollector()
        {
        }

        public event EventHandler<WindowEventArgs> WindowClosed;

        public void OnStart(int eventId, Guid instanceId, DateTime startTime)
        {
            this.EnsureWindow(startTime);
            this.window.Add(eventId, instanceId);
        }

        public void OnEnd(int eventId, Guid instanceId, DateTime endTime)
        {
            this.EnsureWindow(endTime);
            this.window.Complete(eventId, instanceId);
        }

        public void CloseWindow()
        {
            if (this.window != null)
            {
                EventWindow closedWindow = this.window;
                this.window = null;
                EventHandler<WindowEventArgs> handler = this.WindowClosed;
                if (handler != null)
                {
                    handler(this, new WindowEventArgs(closedWindow));
                }
            }
        }

        private void EnsureWindow(DateTime timestamp)
        {
            if (this.window == null)
            {
                this.window = new EventWindow(timestamp);
            }
            else
            {
                TimeSpan delta = timestamp - this.window.StartTime;
                int secondsAfter = (int)delta.TotalSeconds;
                if (secondsAfter > 0)
                {
                    DateTime nextStartTime = this.window.StartTime + TimeSpan.FromSeconds(secondsAfter);
                    EventWindow nextWindow = new EventWindow(this.window, nextStartTime);
                    nextWindow.ClearCompleted();
                    this.CloseWindow();
                    this.window = nextWindow;
                }
            }
        }
    }
}
