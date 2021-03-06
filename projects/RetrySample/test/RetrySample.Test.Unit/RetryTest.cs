﻿//-----------------------------------------------------------------------
// <copyright file="RetryTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RetrySample.Test.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class RetryTest
    {
        public RetryTest()
        {
        }

        [Fact]
        public void Execute_runs_func_once_returns_context()
        {
            int count = 0;
            RetryLoop loop = new RetryLoop(r => Task.FromResult(++count));

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(1, count);
            RetryContext context = task.Result;
            Assert.Equal(1, context.Iteration);
            Assert.NotEqual(TimeSpan.Zero, context.ElapsedTime);
        }

        [Fact]
        public void Execute_runs_func_until_should_not_retry_returns_context()
        {
            int count = 0;
            RetryLoop loop = new RetryLoop(r => Task.FromResult(++count));
            loop.ShouldRetry = r => r.Iteration < 1;
            loop.Succeeded = r => false;

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(2, count);
            RetryContext context = task.Result;
            Assert.Equal(2, context.Iteration);
        }

        [Fact]
        public void Execute_runs_func_returns_context_with_elapsed_time()
        {
            TimeSpan funcElapsed = TimeSpan.MaxValue;
            Func<RetryContext, Task> func = delegate(RetryContext r)
            {
                funcElapsed = r.ElapsedTime;
                return Task.FromResult(false);
            };

            RetryLoop loop = new RetryLoop(func);

            TimeSpan shouldRetryElapsed = TimeSpan.MaxValue;
            loop.ShouldRetry = delegate(RetryContext r)
            {
                shouldRetryElapsed = r.ElapsedTime;
                return false;
            };

            TimeSpan succeededElapsed = TimeSpan.MaxValue;
            loop.Succeeded = delegate(RetryContext r)
            {
                succeededElapsed = r.ElapsedTime;
                return false;
            };

            ElapsedTimerStub timer = new ElapsedTimerStub();
            loop.Timer = timer;

            timer.ElapsedTimes.Enqueue(TimeSpan.FromSeconds(1.0d));
            timer.ElapsedTimes.Enqueue(TimeSpan.FromSeconds(2.5d));
            timer.ElapsedTimes.Enqueue(TimeSpan.FromSeconds(4.0d));
            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(TimeSpan.FromSeconds(1.5d), funcElapsed);
            RetryContext context = task.Result;
            Assert.Equal(1, context.Iteration);
            Assert.Equal(TimeSpan.FromSeconds(3.0d), succeededElapsed);
            Assert.Equal(TimeSpan.FromSeconds(3.0d), shouldRetryElapsed);
            Assert.Equal(TimeSpan.FromSeconds(3.0d), context.ElapsedTime);
        }

        [Fact]
        public void Execute_runs_func_until_succeeded()
        {
            int count = 0;
            RetryLoop loop = new RetryLoop(r => Task.FromResult(++count));
            loop.Succeeded = r => r.Iteration == 1;
            loop.ShouldRetry = r => true;

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(2, context.Iteration);
            Assert.Equal(2, count);
            Assert.True(context.Succeeded);
        }

        [Fact]
        public void Execute_func_throws_sync_exception_caught_and_set_in_context()
        {
            InvalidTimeZoneException exception = new InvalidTimeZoneException("Expected.");
            Func<RetryContext, Task> func = delegate(RetryContext r)
            {
                throw exception;
            };
            RetryLoop loop = new RetryLoop(func);

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(1, context.Iteration);
            Assert.False(context.Succeeded);
            Assert.NotNull(context.Exception);
            Assert.Equal(1, context.Exception.InnerExceptions.Count);
            Assert.Same(exception, context.Exception.InnerExceptions[0]);
        }

        [Fact]
        public void Execute_func_throws_async_exception_caught_and_set_in_context()
        {
            InvalidTimeZoneException exception = new InvalidTimeZoneException("Expected.");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            tcs.SetException(exception);
            RetryLoop loop = new RetryLoop(r => tcs.Task);

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(1, context.Iteration);
            Assert.False(context.Succeeded);
            Assert.NotNull(context.Exception);
            Assert.Equal(1, context.Exception.InnerExceptions.Count);
            Assert.Same(exception, context.Exception.InnerExceptions[0]);
        }

        [Fact]
        public void Execute_invokes_before_retry_func_after_iteration()
        {
            RetryLoop loop = new RetryLoop(r => Task.FromResult(false));
            loop.Succeeded = r => false;
            loop.ShouldRetry = r => r.Iteration < 1;
            int count = 0;
            int iteration = 0;
            loop.BeforeRetry = delegate(RetryContext r)
            {
                ++count;
                iteration = r.Iteration;
                return Task.FromResult(false);
            };

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(2, context.Iteration);
            Assert.False(context.Succeeded);
            Assert.Equal(1, count);
            Assert.Equal(1, iteration);
        }

        [Fact]
        public void Execute_before_retry_throws_sync_exception_caught_and_set_in_context()
        {
            RetryLoop loop = new RetryLoop(r => Task.FromResult(false));
            loop.Succeeded = r => false;
            loop.ShouldRetry = r => r.Iteration < 1;
            InvalidTimeZoneException exception = new InvalidTimeZoneException("Expected.");
            loop.BeforeRetry = delegate(RetryContext r)
            {
                throw exception;
            };

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(2, context.Iteration);
            Assert.False(context.Succeeded);
            Assert.NotNull(context.Exception);
            Assert.Equal(1, context.Exception.InnerExceptions.Count);
            Assert.Same(exception, context.Exception.InnerExceptions[0]);
        }

        [Fact]
        public void Execute_before_retry_throws_async_exception_caught_and_set_in_context()
        {
            RetryLoop loop = new RetryLoop(r => Task.FromResult(false));
            loop.Succeeded = r => false;
            loop.ShouldRetry = r => r.Iteration < 1;
            InvalidTimeZoneException exception = new InvalidTimeZoneException("Expected.");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            tcs.SetException(exception);
            loop.BeforeRetry = r => tcs.Task;

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            Assert.Equal(2, context.Iteration);
            Assert.False(context.Succeeded);
            Assert.NotNull(context.Exception);
            Assert.Equal(1, context.Exception.InnerExceptions.Count);
            Assert.Same(exception, context.Exception.InnerExceptions[0]);
        }

        [Fact]
        public void Can_add_and_get_results_from_context()
        {
            RetryContext context = new RetryContext();
            context.Add("a", 1);
            context.Add("b", "Two");
            object c1 = new object();
            context.Add("c", c1);

            int a2 = context.Get<int>("a");
            string b2 = context.Get<string>("b");
            object c2 = context.Get<object>("c");

            Assert.Equal(1, a2);
            Assert.Equal("Two", b2);
            Assert.Same(c2, c1);

            object x = context.Get<object>("x");
            int y = context.Get<int>("y");
            string z = context.Get<string>("z");

            Assert.Null(x);
            Assert.Equal(0, y);
            Assert.Null(z);
        }

        [Fact]
        public void Execute_exception_cleared_on_next_iteration()
        {
            int nullCount = 0;
            Func<RetryContext, Task> func = delegate(RetryContext r)
            {
                if (r.Exception == null)
                {
                    ++nullCount;
                }

                throw new InvalidTimeZoneException("Expected.");
            };
            RetryLoop loop = new RetryLoop(func);
            loop.ShouldRetry = r => r.Iteration < 2;

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            Assert.Equal(3, nullCount);
        }

        [Fact]
        public void Add_async_extension_awaits_and_adds_result()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            RetryLoop loop = new RetryLoop(r => r.AddAsync("Result", tcs.Task));

            Task<RetryContext> task = loop.ExecuteAsync();

            Assert.False(task.IsCompleted);

            tcs.SetResult("xyz");

            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
            RetryContext context = task.Result;
            string result = context.Get<string>("Result");
            Assert.Equal("xyz", result);
        }

        private sealed class ElapsedTimerStub : IElapsedTimer
        {
            public ElapsedTimerStub()
            {
                this.ElapsedTimes = new Queue<TimeSpan>();
            }

            public Queue<TimeSpan> ElapsedTimes { get; private set; }

            public TimeSpan Elapsed
            {
                get { return this.ElapsedTimes.Dequeue(); }
            }
        }
    }
}
