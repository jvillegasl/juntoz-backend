using StoreDatabase.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderStoreApi.Models;

namespace OrderStoreApi.Services;

public class OrdersService
{
    private readonly IMongoCollection<Order> _ordersCollection;

    public OrdersService(IOptions<StoreDatabaseSettings> storeDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            storeDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            storeDatabaseSettings.Value.DatabaseName);

        _ordersCollection = mongoDatabase.GetCollection<Order>(
            storeDatabaseSettings.Value.OrdersCollectionName
        );
    }

    public async Task<List<Order>> GetAsync() =>
        await _ordersCollection.Find(_ => true).ToListAsync();

    public async Task<Order?> GetAsync(string id) =>
        await _ordersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<int> CreateAsync(NewOrder request)
    {
        var result = await _ordersCollection.Find(x => true).SortByDescending(d => d.OrderNumber).Limit(1).FirstOrDefaultAsync();
        int number = result == null ? 0 : result.OrderNumber + 1;

        Order newOrder = new()
        {
            ClientDNI = request.ClientDNI,
            Description = request.Description,
            OrderNumber = number
        };

        await _ordersCollection.InsertOneAsync(newOrder);

        return 123;
    }

    public async Task UpdateAsync(string id, Order updatedBook) =>
        await _ordersCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _ordersCollection.DeleteOneAsync(x => x.Id == id);
}