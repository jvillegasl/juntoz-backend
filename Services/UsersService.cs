using BCrypt;
using UserStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace UserStoreApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(
        IOptions<UserStoreDatabaseSettings> userStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userStoreDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            userStoreDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<bool> CreateAsync(RegisterUser request)
    {
        var count = await _usersCollection.Find(x => x.Username == request.Username).CountDocumentsAsync();
        if (count > 0)
        {
            return false;
        }

        User newUser = new()
        {
            Username = request.Username,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        await _usersCollection.InsertOneAsync(newUser);

        return true;
    }

    public async Task<User?> FindByUsernameAsync(string? username) =>
        await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();



    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);
}
