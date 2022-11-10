using CartingService.Domain.Entities;
using CartingService.Domain.Repositories;
using LiteDB;
using System;
using System.Collections.Generic;

namespace CartingService.Persistence.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ILiteCollection<Item> _collection;

        public ItemRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<Item>("items");
        }

        public IEnumerable<Item> GetAllByCartId(Guid cartId) => _collection.Find(x => x.CartId == cartId);

        public Item GetById(int itemId) => _collection.FindById(itemId);

        public void Insert(Item item) => _collection.Insert(item);

        public void Update(Item item) => _collection.Update(item);

        public void Remove(int itemId) => _collection.Delete(itemId);
    }
}
