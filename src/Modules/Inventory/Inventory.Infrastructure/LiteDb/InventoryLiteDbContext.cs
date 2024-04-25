namespace Modules.Inventory.Infrastructure.LiteDb;

using BridgingIT.DevKit.Infrastructure.LiteDb.Repositories;
using LiteDB;

public class InventoryLiteDbContext : LiteDbContext
{
    public InventoryLiteDbContext(string connectionString, BsonMapper bsonMapper = null)
        : base(connectionString, bsonMapper)
    {
        //BsonMapper.Global.Entity<Stock>() // mapping example https://www.litedb.org/docs/object-mapping/
        //.Id(x => x.Id)
        //.Ignore(d => d.DomainEvents); // TODO: causes > Value cannot be null. (Parameter 'Member 'DomainEvents' not found in type 'Stock' (use IncludeFields in BsonMapper)') https://github.com/mbdavid/LiteDB/issues/1159
    }
}
