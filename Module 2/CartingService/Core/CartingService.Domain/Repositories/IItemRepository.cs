using CartingService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CartingService.Domain.Repositories
{
    public interface IItemRepository
    {
        IEnumerable<Item> GetAllByCartId(Guid cartId);

        Item GetById(int itemId);

        void Insert(Item item);

        void Update(Item item);

        void Remove(int itemId);
    }
}
