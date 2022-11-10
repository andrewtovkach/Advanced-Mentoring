using System;

namespace CartingService.Domain.Exceptions
{
    public class CartNotFoundException : NotFoundException
    {
        public CartNotFoundException(Guid cartId)
            : base($"The cart with the identifier {cartId} was not found.")
        {
        }
    }
}
