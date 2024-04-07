﻿using Medallion.Threading.SqlServer;
using NUnit.Framework;

namespace Medallion.Threading.Tests.SqlServer;

[Category("CI")]
public class SqlConnectionOptionsBuilderTest
{
    [Test]
    public void TestValidatesArguments()
    {
        var builder = new SqlConnectionOptionsBuilder();
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.KeepaliveCadence(TimeSpan.FromMilliseconds(-2)));
        Assert.Throws<ArgumentOutOfRangeException>(() => builder.KeepaliveCadence(TimeSpan.MaxValue));

        Assert.Throws<ArgumentException>(() => SqlConnectionOptionsBuilder.GetOptions(o => o.UseMultiplexing().UseTransaction()));
    }

    [Test]
    public void TestDefaults()
    {
        var options = SqlConnectionOptionsBuilder.GetOptions(null);
        options.keepaliveCadence.ShouldEqual(TimeSpan.FromMinutes(10));
        Assert.That(options.useMultiplexing, Is.True);
        Assert.That(options.useTransaction, Is.False);
        options.ShouldEqual(SqlConnectionOptionsBuilder.GetOptions(o => { }));
    }

    [Test]
    public void TestUseTransactionDoesNotRequireDisablingMultiplexing()
    {
        var options = SqlConnectionOptionsBuilder.GetOptions(o => o.UseTransaction());
        Assert.That(options.useTransaction, Is.True);
        Assert.That(options.useMultiplexing, Is.False);
    }
}
