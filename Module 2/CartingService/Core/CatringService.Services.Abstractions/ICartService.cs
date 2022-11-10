using CartingService.Contracts;
using System;
using System.Collections.Generic;

namespace CatringService.Services.Abstractions
{
    public interface ICartService
    {
        IEnumerable<CartDto> GetAll();

        CartDto GetById(Guid cartId);

        CartDto Create(CreateCartDto cartForCreationDto);

        void Delete(Guid cartId);
    }
}
