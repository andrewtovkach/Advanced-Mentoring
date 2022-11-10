using System;

namespace CartingService.Domain.Exceptions
{
    public class ItemDoesNotBelongToCartException : BadRequestException
    {
        public ItemDoesNotBelongToCartException(Guid cartId, int itemId)
            : base($"The item with the identifier {itemId} does not belong to the cart with the identifier {cartId}")
        {
        }
    }
}
