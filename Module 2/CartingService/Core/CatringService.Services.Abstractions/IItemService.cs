using CartingService.Contracts;
using System;
using System.Collections.Generic;

namespace CatringService.Services.Abstractions
{
    public interface IItemService
    {
        IEnumerable<ItemDto> GetAllByCartId(Guid cartId);

        ItemDto GetById(Guid cartId, int itemId);

        ItemDto Create(Guid cartId, CreateItemDto itemForCreationDto);

        void Update(Guid cartId, UpdateItemDto itemForUpdateDto);

        void Delete(Guid cartId, int itemId);
    }
}
