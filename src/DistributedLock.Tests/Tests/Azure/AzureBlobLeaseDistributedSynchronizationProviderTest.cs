﻿using Azure.Storage.Blobs;
using Medallion.Threading.Azure;
using NUnit.Framework;

namespace Medallion.Threading.Tests.Azure;

public class AzureBlobLeaseDistributedSynchronizationProviderTest
{
    [Test]
    public void TestArgumentValidation()
    {
        Assert.Throws<ArgumentNullException>(() => new AzureBlobLeaseDistributedSynchronizationProvider(null!));
    }

    [Test]
    public async Task BasicTest()
    {
        const string LockName = TargetFramework.Current + "ProviderBasicTest";

        var container = new BlobContainerClient(AzureCredentials.ConnectionString, AzureCredentials.DefaultBlobContainerName);
        var provider = new AzureBlobLeaseDistributedSynchronizationProvider(container);
        await using (await provider.AcquireLockAsync(LockName))
        {
            await using var handle = await provider.TryAcquireLockAsync(LockName);
            Assert.That(handle, Is.Null);
        }
    }
}
