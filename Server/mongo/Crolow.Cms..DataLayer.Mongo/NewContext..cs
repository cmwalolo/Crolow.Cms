using Crolow.Cms.DataLayer.Mongo;
using Crolow.Cms.Server.Core.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Crolow.Cms.DataLayer.Mongo;
public interface IMongoUnitOfWork : IAsyncDisposable
{
    IClientSessionHandle Session { get; }

    Task BeginAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
public class MongoUnitOfWork : IMongoUnitOfWork
{
    private readonly IMongoClient _client;
    public IClientSessionHandle Session { get; private set; } = null!;

    public MongoUnitOfWork(IMongoClient client)
    {
        _client = client;
    }

    public async Task BeginAsync(CancellationToken ct = default)
    {
        Session = await _client.StartSessionAsync(cancellationToken: ct);
        Session.StartTransaction();
    }

    public Task CommitAsync(CancellationToken ct = default)
        => Session.CommitTransactionAsync(ct);

    public Task RollbackAsync(CancellationToken ct = default)
        => Session.AbortTransactionAsync(ct);

    public ValueTask DisposeAsync()
    {
        Session?.Dispose();
        return ValueTask.CompletedTask;
    }
}

public class TransactionalContext : ITransactionalContext
{
    public IClientSessionHandle Session { get; }

    public TransactionalContext(IClientSessionHandle session)
    {
        Session = session;
    }
}

public interface ITransactionalContext
{
    public IClientSessionHandle Session { get; }

}

public interface IMongoClientProvider
{
    IMongoClient Client { get; }
}

public class MongoClientProvider : IMongoClientProvider
{
    public IMongoClient Client { get; }

    public MongoClientProvider(string connectionString)
    {
        Client = new MongoClient(connectionString);
    }
}

public abstract class MongoDbContext
{
    protected readonly IMongoClient Client;
    public IMongoDatabase Database { get; }

    protected MongoDbContext(IMongoClient client, string databaseName)
    {
        Client = client;
        Database = client.GetDatabase(databaseName);
    }
}


public class MetaDbContext : MongoDbContext
{
    public MetaDbContext(IMongoClient client)
        : base(client, "MetaDatabaseName") // or pass name from config
    {
    }

    public IMongoCollection<NodeDefinition> Nodes
        => Database.GetCollection<NodeDefinition>("Nodes");

    //public IMongoCollection<VersionInfo> Versions
    //    => Database.GetCollection<VersionInfo>("Versions");

    //public IMongoCollection<Translation> Translations
    //    => Database.GetCollection<Translation>("Translations");

    //public IMongoCollection<Relation> Relations
    //    => Database.GetCollection<Relation>("Relations");
}

public class DataDbContext
{
    private readonly MetaDbContext _metaDbContext;
    public DataDbContext(MetaDbContext context)
    {
        _metaDbContext = context;
    }
}

public class ItemManager
{
    private readonly MetaDbContext _metaDbContext;

    public ItemManager(
        MetaDbContext metaDbContext)
    {
        _metaDbContext = metaDbContext;

    }

}

public class MongoTransactionRunner
{
    private readonly IMongoConnectionFactory _connections;

    public MongoTransactionRunner(IMongoConnectionFactory connections)
    {
        _connections = connections;
    }

    public async Task RunAsync(ConnectionId connection, Func<IClientSessionHandle, Task> action)
    {
        var client = _connections.GetClient(connection);

        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            await action(session);
            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}

public interface IMongoConnectionFactory
{
    IMongoClient GetClient(ConnectionId connection);
}

public class MongoConnectionFactory : IMongoConnectionFactory
{
    private readonly ConcurrentDictionary<ConnectionId, IMongoClient> _clients = new();

    public MongoConnectionFactory(IReadOnlyDictionary<ConnectionId, string> connectionStrings)
    {
        foreach (var kvp in connectionStrings)
        {
            _clients[kvp.Key] = new MongoClient(kvp.Value);
        }
    }

    public IMongoClient GetClient(ConnectionId connection)
        => _clients[connection];
}
}


public sealed class StoreDescriptor
{
    public ObjectId StoreId { get; init; }
    public ConnectionId Connection { get; init; }
    public string Database { get; init; }
    public string Collection { get; init; }
}

public interface IStoreResolver
{
    StoreDescriptor GetDescriptor(ObjectId storeId);
    IMongoCollection<T> GetCollection<T>(ObjectId storeId, IClientSessionHandle session = null);
}

public class StoreResolver : IStoreResolver
{
    private readonly IMongoConnectionFactory _connectionFactory;
    private readonly Dictionary<ObjectId, StoreDescriptor> _stores;

    public StoreResolver(
        IMongoConnectionFactory connectionFactory,
        IEnumerable<StoreDescriptor> templatesFromCoreDb)
    {
        _connectionFactory = connectionFactory;
        _stores = templatesFromCoreDb
            .Select(t => new StoreDescriptor
            {
                StoreId = t.StoreId,
                Connection = t.Connection,
                Database = t.Database,
                Collection = t.Collection
            })
            .ToDictionary(x => x.StoreId, x => x);
    }

    public StoreDescriptor GetDescriptor(ObjectId storeId)
        => _stores[storeId];

    public IMongoCollection<T> GetCollection<T>(ObjectId storeId, IClientSessionHandle session = null)
    {
        var desc = _stores[storeId];
        var client = _connectionFactory.GetClient(desc.Connection);
        var db = client.GetDatabase(desc.Database);
        return db.GetCollection<T>(desc.Collection);
    }
}

