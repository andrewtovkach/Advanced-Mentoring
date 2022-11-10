using CartingService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CartingService.Domain.Repositories
{
    public interface ICartRepository
    {
        IEnumerable<Cart> GetAll();

        Cart GetById(Guid cartId);

        void Insert(Cart cart);

        void Remove(Guid cartId);
    }
}
