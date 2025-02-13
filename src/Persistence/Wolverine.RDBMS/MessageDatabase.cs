using System;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.Logging;
using Weasel.Core;
using Weasel.Core.Migrations;
using Wolverine.Persistence.Durability;
using Wolverine.RDBMS.Durability;
using Wolverine.Runtime;
using Wolverine.Runtime.WorkerQueues;
using Wolverine.Transports.Local;

namespace Wolverine.RDBMS;

public abstract partial class MessageDatabase<T> : DatabaseBase<T>,
    IMessageDatabase, IMessageStoreAdmin where T : DbConnection, new()
{
    protected readonly CancellationToken _cancellation;
    private readonly string _outgoingEnvelopeSql;

    protected MessageDatabase(DatabaseSettings databaseSettings, DurabilitySettings settings,
        ILogger logger) : base(new MigrationLogger(logger), AutoCreate.CreateOrUpdate, databaseSettings.Migrator,
        "WolverineEnvelopeStorage", databaseSettings.ConnectionString!)
    {
        Settings = databaseSettings;

        Durability = settings;
        _cancellation = settings.Cancellation;

        var transaction = new DurableStorageSession(databaseSettings, settings.Cancellation, logger);

        Session = transaction;

        _cancellation = settings.Cancellation;
        _deleteIncomingEnvelopeById =
            $"update {Settings.SchemaName}.{DatabaseConstants.IncomingTable} set {DatabaseConstants.Status} = '{EnvelopeStatus.Handled}', {DatabaseConstants.KeepUntil} = @keepUntil where id = @id";
        _incrementIncominEnvelopeAttempts =
            $"update {Settings.SchemaName}.{DatabaseConstants.IncomingTable} set attempts = @attempts where id = @id";

        // ReSharper disable once VirtualMemberCallInConstructor
        _outgoingEnvelopeSql = determineOutgoingEnvelopeSql(databaseSettings, settings);


    }

    public DurabilitySettings Durability { get; }

    public DatabaseSettings Settings { get; }

    public IMessageStoreAdmin Admin => this;

    public IDurableStorageSession Session { get; }

    public abstract void Describe(TextWriter writer);

    public async Task ReleaseIncomingAsync(int ownerId)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync(_cancellation);

        await conn
            .CreateCommand(
                $"update {Settings.SchemaName}.{DatabaseConstants.IncomingTable} set owner_id = 0 where owner_id = @owner")
            .With("owner", ownerId)
            .ExecuteNonQueryAsync(_cancellation);
    }

    public async Task ReleaseIncomingAsync(int ownerId, Uri receivedAt)
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync(_cancellation);

        var impacted = await conn
            .CreateCommand(
                $"update {Settings.SchemaName}.{DatabaseConstants.IncomingTable} set owner_id = 0 where owner_id = @owner and {DatabaseConstants.ReceivedAt} = @uri")
            .With("owner", ownerId)
            .With("uri", receivedAt.ToString())
            .ExecuteNonQueryAsync(_cancellation);
    }

    public IDurabilityAgent BuildDurabilityAgent(IWolverineRuntime runtime, IContainer container)
    {
        var durabilityLogger = container.GetInstance<ILogger<DurabilityAgent>>();

        // TODO -- use the worker queue for Retries?
        var worker = new DurableReceiver(new LocalQueue("scheduled"), runtime, runtime.Pipeline);
        return new DurabilityAgent(runtime, runtime.Logger, durabilityLogger, worker, this,
            runtime.Options.Durability, Settings);
    }

    public void Dispose()
    {
        Session?.Dispose();
    }
}