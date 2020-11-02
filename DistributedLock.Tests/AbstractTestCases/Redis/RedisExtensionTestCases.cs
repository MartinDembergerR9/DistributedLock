﻿using Medallion.Threading.Tests.Redis;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Threading.Tests.Redis
{
    public abstract class RedisExtensionTestCases<TLockProvider, TDatabaseProvider>
        where TLockProvider : TestingLockProvider<TestingRedisSynchronizationStrategy<TDatabaseProvider>>, new()
        where TDatabaseProvider : TestingRedisDatabaseProvider, new()
    {
        private TLockProvider _provider = default!;

        [SetUp]
        public void SetUp() => this._provider = new TLockProvider();

        [TearDown]
        public void TearDown() => this._provider.Dispose();

        [Test]
        [NonParallelizable] // timing-sensitive
        public async Task TestCanExtendLock()
        {
            this._provider.Strategy.SetOptions(o => o.Expiry(TimeSpan.FromSeconds(0.3)).BusyWaitSleepTime(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5)));
            var @lock = this._provider.CreateLock("lock");

            await using var handle = await @lock.AcquireAsync();

            var secondHandleTask = @lock.AcquireAsync().AsTask();
            _ = secondHandleTask.ContinueWith(t => t.Result.Dispose()); // ensure cleanup
            Assert.IsFalse(await secondHandleTask.WaitAsync(TimeSpan.FromSeconds(.5)));

            await handle.DisposeAsync();

            Assert.IsTrue(await secondHandleTask.WaitAsync(TimeSpan.FromSeconds(5)));
        }
    }
}
