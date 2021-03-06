﻿//-----------------------------------------------------------------------
// <copyright file="AsyncOperationTest.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AsyncEnum35Sample.Test.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Xunit;

    public class AsyncOperationTest
    {
        public AsyncOperationTest()
        {
        }

        [Fact]
        public void Set_result_in_ctor_and_break_completes_sync()
        {
            SetResultInCtorOperation op = new SetResultInCtorOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SetResultInCtorOperation.End(result));
        }

        [Fact]
        public void Set_result_in_enumerator_and_break_completes_sync()
        {
            SetResultInEnumeratorOperation op = new SetResultInEnumeratorOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SetResultInEnumeratorOperation.End(result));
        }

        [Fact]
        public void Set_result_in_finally_and_break_completes_sync()
        {
            SetResultInFinallyOperation op = new SetResultInFinallyOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SetResultInFinallyOperation.End(result));
        }

        [Fact]
        public void Set_result_after_one_sync_step_completes_sync()
        {
            SetResultAfterOneStepOperation op = new SetResultAfterOneStepOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SetResultAfterOneStepOperation.End(result));
        }

        [Fact]
        public void Throw_before_yield_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowBeforeYieldOperation op = new ThrowBeforeYieldOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throw_after_one_sync_step_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowAfterOneStepOperation op = new ThrowAfterOneStepOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throw_in_finally_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowInFinallyOperation op = new ThrowInFinallyOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Set_result_during_one_sync_step_completes_sync()
        {
            SetResultDuringOneStepOperation op = new SetResultDuringOneStepOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SetResultDuringOneStepOperation.End(result));
        }

        [Fact]
        public void Throw_during_one_sync_step_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowDuringOneStepOperation op = new ThrowDuringOneStepOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throw_on_end_of_one_sync_step_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowOnEndOfOneStepOperation op = new ThrowOnEndOfOneStepOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Completes_async_after_one_async_step()
        {
            OneAsyncStepOperation op = new OneAsyncStepOperation();
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete(1234);

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            Assert.Equal(1234, OneAsyncStepOperation.End(result));
        }

        [Fact]
        public void Completes_with_async_exception_after_one_async_step_with_async_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            OneAsyncStepOperation op = new OneAsyncStepOperation();
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete(expected);
            
            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => OneAsyncStepOperation.End(result));
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Completes_with_async_exception_after_two_async_steps_with_async_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            TwoAsyncStepOperation op = new TwoAsyncStepOperation();
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete(1234);

            Assert.False(result.IsCompleted);

            op.Complete(expected);

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => TwoAsyncStepOperation.End(result));
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Completes_with_async_exception_after_one_async_step_and_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowAfterAsyncStepOperation op = new ThrowAfterAsyncStepOperation(expected);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete();

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => ThrowAfterAsyncStepOperation.End(result));
            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throws_sync_and_runs_finally_after_one_step_throws_sync()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowDuringAsyncStepWithFinallyOperation op = new ThrowDuringAsyncStepWithFinallyOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
            Assert.True(op.RanFinally);
        }

        [Fact]
        public void Completes_async_and_runs_finally_after_one_step_completes_async()
        {
            OneAsyncStepWithFinallyOperation op = new OneAsyncStepWithFinallyOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete(null);

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            Assert.Equal(1234, OneAsyncStepWithFinallyOperation.End(result));
        }

        [Fact]
        public void Completes_with_async_exception_and_runs_finally_after_one_step_completes_with_async_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            OneAsyncStepWithFinallyOperation op = new OneAsyncStepWithFinallyOperation(1234);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete(expected);

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => OneAsyncStepWithFinallyOperation.End(result));
            Assert.Same(expected, actual);
            Assert.Equal(1234, op.ResultAccessor);
        }

        [Fact]
        public void Completes_with_async_exception_and_runs_finally_after_one_step_completes_async_then_throws_sync_exception_on_end()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowOnEndOfOneAsyncStepWithFinallyOperation op = new ThrowOnEndOfOneAsyncStepWithFinallyOperation(expected);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete();

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => ThrowOnEndOfOneAsyncStepWithFinallyOperation.End(result));
            Assert.Same(expected, actual);
            Assert.True(op.RanFinally);
        }

        [Fact]
        public void Completes_with_async_exception_and_runs_finally_after_one_step_completes_async_then_throws_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowAfterAsyncStepWithFinallyOperation op = new ThrowAfterAsyncStepWithFinallyOperation(expected);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete();

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => ThrowAfterAsyncStepWithFinallyOperation.End(result));
            Assert.Same(expected, actual);
            Assert.True(op.RanFinally);
        }

        [Fact]
        public void Multiple_successive_sync_completion_does_not_cause_infinite_stack_recursion()
        {
            MultipleSyncCompletionOperation op = new MultipleSyncCompletionOperation(1000);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            MultipleSyncCompletionOperation.End(result);
        }

        [Fact]
        public void Invokes_callback_on_sync_completion()
        {
            SetResultAfterOneStepOperation op = new SetResultAfterOneStepOperation(1234);
            object state = new object();
            IAsyncResult returnedResult = null;
            IAsyncResult callbackResult = null;
            AsyncCallback callback = delegate(IAsyncResult r)
            {
                callbackResult = r;
                Assert.Null(returnedResult);
                Assert.Same(state, r.AsyncState);
                Assert.Equal(1234, SetResultAfterOneStepOperation.End(r));
            };

            returnedResult = op.Start(callback, state);

            Assert.NotNull(callbackResult);
            Assert.True(returnedResult.IsCompleted);
            Assert.True(returnedResult.CompletedSynchronously);
            Assert.Same(callbackResult, returnedResult);
        }

        [Fact]
        public void Invokes_callback_on_async_completion()
        {
            OneAsyncStepOperation op = new OneAsyncStepOperation();
            object state = new object();
            IAsyncResult returnedResult = null;
            IAsyncResult callbackResult = null;
            AsyncCallback callback = delegate(IAsyncResult r)
            {
                callbackResult = r;
                Assert.NotNull(returnedResult);
                Assert.Same(state, r.AsyncState);
                Assert.Equal(1234, OneAsyncStepOperation.End(r));
            };

            returnedResult = op.Start(callback, state);

            Assert.Null(callbackResult);
            Assert.False(returnedResult.IsCompleted);

            op.Complete(1234);

            Assert.True(returnedResult.IsCompleted);
            Assert.False(returnedResult.CompletedSynchronously);
            Assert.Same(callbackResult, returnedResult);
        }

        [Fact]
        public void Completes_successfully_after_catching_and_handling_exception_on_begin()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowOnBeginAndHandleOperation op = new ThrowOnBeginAndHandleOperation(1234, expected);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, ThrowOnBeginAndHandleOperation.End(result));
            Assert.Same(expected, op.CaughtException);
        }

        [Fact]
        public void Completes_successfully_after_catching_and_handling_exception_on_end()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowOnEndAndHandleOperation op = new ThrowOnEndAndHandleOperation(1234, expected);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, ThrowOnEndAndHandleOperation.End(result));
            Assert.Same(expected, op.CaughtException);
        }

        [Fact]
        public void Throws_sync_no_matching_exception_handler_on_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            NoMatchingHandlerSyncOperation op = new NoMatchingHandlerSyncOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Completes_successfully_on_matching_exception_handler_on_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            SecondMatchingHandlerSyncOperation op = new SecondMatchingHandlerSyncOperation(1234, expected);
            IAsyncResult result = op.Start(null, null);

            Assert.True(result.IsCompleted);
            Assert.True(result.CompletedSynchronously);
            Assert.Equal(1234, SecondMatchingHandlerSyncOperation.End(result));
            Assert.Same(expected, op.CaughtException);
        }

        [Fact]
        public void Throws_sync_all_handlers_return_false_on_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ReturnFalseHandlersSyncOperation op = new ReturnFalseHandlersSyncOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throws_sync_on_throw_from_handler_on_sync_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowFromHandlerSyncOperation op = new ThrowFromHandlerSyncOperation(expected);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => op.Start(null, null));

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Throws_async_on_throw_from_handler_on_async_exception()
        {
            InvalidTimeZoneException expected = new InvalidTimeZoneException("Expected.");
            ThrowFromHandlerAsyncOperation op = new ThrowFromHandlerAsyncOperation(expected);
            IAsyncResult result = op.Start(null, null);

            Assert.False(result.IsCompleted);

            op.Complete();

            Assert.True(result.IsCompleted);
            Assert.False(result.CompletedSynchronously);
            InvalidTimeZoneException actual = Assert.Throws<InvalidTimeZoneException>(() => ThrowFromHandlerAsyncOperation.End(result));
            Assert.Same(expected, actual);
        }

        private abstract class TestAsyncOperation : AsyncOperation<int>
        {
            protected TestAsyncOperation()
            {
            }
        }

        private sealed class SetResultInCtorOperation : TestAsyncOperation
        {
            public SetResultInCtorOperation(int result)
            {
                this.Result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield break;
            }
        }

        private sealed class SetResultInEnumeratorOperation : TestAsyncOperation
        {
            private readonly int result;

            public SetResultInEnumeratorOperation(int result)
            {
                this.result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                this.Result = this.result;
                yield break;
            }
        }

        private sealed class SetResultInFinallyOperation : TestAsyncOperation
        {
            private readonly int result;

            public SetResultInFinallyOperation(int result)
            {
                this.result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield break;
                }
                finally
                {
                    this.Result = this.result;
                }
            }
        }

        private sealed class SetResultAfterOneStepOperation : TestAsyncOperation
        {
            private readonly int result;

            public SetResultAfterOneStepOperation(int result)
            {
                this.result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    false,
                    (t, c, s) => new CompletedAsyncResult<bool>(t, c, s),
                    (t, r) => CompletedAsyncResult<bool>.End(r));
                this.Result = this.result;
            }
        }

        private sealed class ThrowBeforeYieldOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowBeforeYieldOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                throw this.exception;
            }
        }

        private sealed class ThrowAfterOneStepOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowAfterOneStepOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    false,
                    (t, c, s) => new CompletedAsyncResult<bool>(t, c, s),
                    (t, r) => CompletedAsyncResult<bool>.End(r));
                throw this.exception;
            }
        }

        private sealed class ThrowInFinallyOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowInFinallyOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield break;
                }
                finally
                {
                    throw this.exception;
                }
            }
        }

        private sealed class SetResultDuringOneStepOperation : TestAsyncOperation
        {
            private readonly int result;

            public SetResultDuringOneStepOperation(int result)
            {
                this.result = result;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => new CompletedAsyncResult<int>(thisPtr.result, c, s),
                    (thisPtr, r) => thisPtr.Result = CompletedAsyncResult<int>.End(r));
            }
        }

        private sealed class ThrowDuringOneStepOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowDuringOneStepOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.ThrowSync(),
                    (thisPtr, r) => thisPtr.Invalid());
            }

            private IAsyncResult ThrowSync()
            {
                throw this.exception;
            }

            private void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }
        }

        private sealed class ThrowOnEndOfOneStepOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowOnEndOfOneStepOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => new CompletedAsyncResult<int>(1, c, s),
                    (thisPtr, r) => thisPtr.ThrowSync(CompletedAsyncResult<int>.End(r)));
            }

            private void ThrowSync(int result)
            {
                if (result == 1)
                {
                    throw this.exception;
                }
            }
        }

        private sealed class OneAsyncStepOperation : TestAsyncOperation
        {
            private AsyncResult<int> result;

            public OneAsyncStepOperation()
            {
            }

            public void Complete(int result)
            {
                this.result.SetAsCompleted(result, false);
            }

            public void Complete(Exception exception)
            {
                this.result.SetAsCompleted(exception, false);
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.result = new AsyncResult<int>(c, s),
                    (thisPtr, r) => thisPtr.Result = ((AsyncResult<int>)r).EndInvoke());
            }
        }

        private sealed class TwoAsyncStepOperation : TestAsyncOperation
        {
            private AsyncResult<int> result;

            public TwoAsyncStepOperation()
            {
            }

            public void Complete(int result)
            {
                this.result.SetAsCompleted(result, false);
            }

            public void Complete(Exception exception)
            {
                this.result.SetAsCompleted(exception, false);
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.result = new AsyncResult<int>(c, s),
                    (thisPtr, r) => thisPtr.Result = ((AsyncResult<int>)r).EndInvoke());
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.result = new AsyncResult<int>(c, s),
                    (thisPtr, r) => thisPtr.Result = ((AsyncResult<int>)r).EndInvoke());
            }
        }

        private sealed class ThrowAfterAsyncStepOperation : TestAsyncOperation
        {
            private readonly Exception exception;
            private AsyncResult<bool> result;

            public ThrowAfterAsyncStepOperation(Exception exception)
            {
                this.exception = exception;
            }

            public void Complete()
            {
                this.result.SetAsCompleted(false, false);
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.result = new AsyncResult<bool>(c, s),
                    (thisPtr, r) => ((AsyncResult<bool>)r).EndInvoke());
                throw this.exception;
            }
        }

        private sealed class ThrowDuringAsyncStepWithFinallyOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowDuringAsyncStepWithFinallyOperation(Exception exception)
            {
                this.exception = exception;
            }

            public bool RanFinally { get; private set; }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield return Step.Await(
                        this,
                        (thisPtr, c, s) => thisPtr.Throw(),
                        (thisPtr, r) => Invalid());
                }
                finally
                {
                    this.RanFinally = true;
                }
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private IAsyncResult Throw()
            {
                throw this.exception;
            }
        }

        private sealed class OneAsyncStepWithFinallyOperation : TestAsyncOperation
        {
            private readonly int result;

            private AsyncResult<bool> asyncResult;

            public OneAsyncStepWithFinallyOperation(int result)
            {
                this.result = result;
            }

            public int ResultAccessor
            {
                get { return this.Result; }
            }

            public void Complete(Exception exception)
            {
                if (exception == null)
                {
                    this.asyncResult.SetAsCompleted(false, false);
                }
                else
                {
                    this.asyncResult.SetAsCompleted(exception, false);
                }
            }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield return Step.Await(
                        this,
                        (thisPtr, c, s) => thisPtr.asyncResult = new AsyncResult<bool>(c, s),
                        (thisPtr, r) => ((AsyncResult<bool>)r).EndInvoke());
                }
                finally
                {
                    this.Result = this.result;
                }
            }
        }

        private sealed class ThrowOnEndOfOneAsyncStepWithFinallyOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            private AsyncResult<int> result;

            public ThrowOnEndOfOneAsyncStepWithFinallyOperation(Exception exception)
            {
                this.exception = exception;
            }

            public bool RanFinally { get; private set; }

            public void Complete()
            {
                this.result.SetAsCompleted(this.exception, false);
            }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield return Step.Await(
                        this,
                        (thisPtr, c, s) => thisPtr.result = new AsyncResult<int>(c, s),
                        (thisPtr, r) => ((AsyncResult<int>)r).EndInvoke());
                }
                finally
                {
                    this.RanFinally = true;
                }
            }
        }

        private sealed class ThrowAfterAsyncStepWithFinallyOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            private AsyncResult<bool> result;

            public ThrowAfterAsyncStepWithFinallyOperation(Exception exception)
            {
                this.exception = exception;
            }

            public bool RanFinally { get; private set; }

            public void Complete()
            {
                this.result.SetAsCompleted(false, false);
            }

            protected override IEnumerator<Step> Steps()
            {
                try
                {
                    yield return Step.Await(
                        this,
                        (thisPtr, c, s) => thisPtr.result = new AsyncResult<bool>(c, s),
                        (thisPtr, r) => ((AsyncResult<bool>)r).EndInvoke());
                    throw this.exception;
                }
                finally
                {
                    this.RanFinally = true;
                }
            }
        }

        private sealed class MultipleSyncCompletionOperation : TestAsyncOperation
        {
            private readonly int iterationCount;

            public MultipleSyncCompletionOperation(int iterationCount)
            {
                this.iterationCount = iterationCount;
            }

            protected override IEnumerator<Step> Steps()
            {
                for (int i = 0; i < this.iterationCount; ++i)
                {
                    yield return Step.Await(
                        false, 
                        (t, c, s) => new CompletedAsyncResult<bool>(t, c, s),
                        (t, r) => CompletedAsyncResult<bool>.End(r));

                    StackTrace stackTrace = new StackTrace(false);
                    if (stackTrace.FrameCount > 100)
                    {
                        throw new InvalidOperationException("Stack too deep!");
                    }
                }
            }
        }

        private sealed class ThrowOnBeginAndHandleOperation : TestAsyncOperation
        {
            private readonly int result;
            private readonly Exception exception;

            public ThrowOnBeginAndHandleOperation(int result, Exception exception)
            {
                this.result = result;
                this.exception = exception;
            }

            public Exception CaughtException { get; private set; }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.Throw(),
                    (thisPtr, r) => Invalid(),
                    Catch<Exception>.AndHandle(this, (thisPtr, e) => thisPtr.Handle(e)));
                this.Result = this.result;
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private IAsyncResult Throw()
            {
                throw this.exception;
            }

            private bool Handle(Exception caughtException)
            {
                this.CaughtException = caughtException;
                return true;
            }
        }

        private sealed class ThrowOnEndAndHandleOperation : TestAsyncOperation
        {
            private readonly int result;
            private readonly Exception exception;

            public ThrowOnEndAndHandleOperation(int result, Exception exception)
            {
                this.result = result;
                this.exception = exception;
            }

            public Exception CaughtException { get; private set; }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => new CompletedAsyncResult<bool>(false, c, s),
                    (thisPtr, r) => thisPtr.EndAndThrow(r),
                    Catch<Exception>.AndHandle(this, (thisPtr, e) => thisPtr.Handle(e)));
                this.Result = this.result;
            }

            private IAsyncResult EndAndThrow(IAsyncResult result)
            {
                ((CompletedAsyncResult<bool>)result).EndInvoke();
                throw this.exception;
            }

            private bool Handle(Exception caughtException)
            {
                this.CaughtException = caughtException;
                return true;
            }
        }

        private sealed class NoMatchingHandlerSyncOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public NoMatchingHandlerSyncOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.Throw(),
                    (thisPtr, r) => Invalid(),
                    Catch<ArgumentException>.AndHandle(this, (thisPtr, e) => true));
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private IAsyncResult Throw()
            {
                throw this.exception;
            }
        }

        private sealed class SecondMatchingHandlerSyncOperation : TestAsyncOperation
        {
            private readonly int result;
            private readonly Exception exception;

            public SecondMatchingHandlerSyncOperation(int result, Exception exception)
            {
                this.result = result;
                this.exception = exception;
            }

            public Exception CaughtException { get; private set; }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.Throw(),
                    (thisPtr, r) => Invalid(),
                    Catch<ArgumentException>.AndHandle(this, (thisPtr, e) => true),
                    Catch<InvalidTimeZoneException>.AndHandle(this, (thisPtr, e) => thisPtr.Handle(e)));
                this.Result = this.result;
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private IAsyncResult Throw()
            {
                throw this.exception;
            }

            private bool Handle(InvalidTimeZoneException e)
            {
                this.CaughtException = e;
                return true;
            }
        }

        private sealed class ReturnFalseHandlersSyncOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ReturnFalseHandlersSyncOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.Throw(),
                    (thisPtr, r) => Invalid(),
                    Catch<Exception>.AndHandle(this, (thisPtr, e) => false),
                    Catch<InvalidTimeZoneException>.AndHandle(this, (thisPtr, e) => false));
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private IAsyncResult Throw()
            {
                throw this.exception;
            }
        }

        private sealed class ThrowFromHandlerSyncOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            public ThrowFromHandlerSyncOperation(Exception exception)
            {
                this.exception = exception;
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => Throw(),
                    (thisPtr, r) => Invalid(),
                    Catch<ArgumentException>.AndHandle(this, (thisPtr, e) => thisPtr.ThrowFromHandler()));
            }

            private static void Invalid()
            {
                throw new InvalidOperationException("This shouldn't happen.");
            }

            private static IAsyncResult Throw()
            {
                throw new ArgumentException("Shouldn't see this.");
            }

            private bool ThrowFromHandler()
            {
                throw this.exception;
            }
        }

        private sealed class ThrowFromHandlerAsyncOperation : TestAsyncOperation
        {
            private readonly Exception exception;

            private AsyncResult<bool> result;

            public ThrowFromHandlerAsyncOperation(Exception exception)
            {
                this.exception = exception;
            }

            public void Complete()
            {
                this.result.SetAsCompleted(new ArgumentException("Shouldn't see this."), false);
            }

            protected override IEnumerator<Step> Steps()
            {
                yield return Step.Await(
                    this,
                    (thisPtr, c, s) => thisPtr.result = new AsyncResult<bool>(c, s),
                    (thisPtr, r) => ((AsyncResult<bool>)thisPtr.result).EndInvoke(),
                    Catch<ArgumentException>.AndHandle(this, (thisPtr, e) => thisPtr.ThrowFromHandler()));
            }

            private bool ThrowFromHandler()
            {
                throw this.exception;
            }
        }
    }
}
