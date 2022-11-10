using System;

namespace CartingService.Domain.Repositories
{
    public interface IRepositoryManager : IDisposable
    {
        ICartRepository CartRepository { get; }

        IItemRepository ItemRepository { get; }
    }
}
