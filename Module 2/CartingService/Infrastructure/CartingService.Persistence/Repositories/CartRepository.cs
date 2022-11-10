using CartingService.Domain.Entities;
using CartingService.Domain.Repositories;
using LiteDB;
using System;
using System.Collections.Generic;

namespace CartingService.Persistence.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ILiteCollection<Cart> _collection;

        public CartRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<Cart>("carts");
        }

        public IEnumerable<Cart> GetAll() => _collection.FindAll();

        public Cart GetById(Guid cartId) => _collection.FindById(cartId);

        public void Insert(Cart cart) => _collection.Insert(cart);

        public void Remove(Guid cartId) => _collection.Delete(cartId);
    }
}
