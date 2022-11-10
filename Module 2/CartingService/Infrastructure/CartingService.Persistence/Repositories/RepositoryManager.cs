using CartingService.Domain.Repositories;
using LiteDB;
using System;

namespace CartingService.Persistence.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<ICartRepository> _lazyCartRepository;
        private readonly Lazy<IItemRepository> _lazyItemRepository;
        private readonly LiteDatabase _db;
        private bool disposedValue;

        public RepositoryManager(string connectionString)
        {
            _db = new LiteDatabase(connectionString);

            _lazyCartRepository = new Lazy<ICartRepository>(() => new CartRepository(_db));
            _lazyItemRepository = new Lazy<IItemRepository>(() => new ItemRepository(_db));
        }

        public ICartRepository CartRepository => _lazyCartRepository.Value;

        public IItemRepository ItemRepository => _lazyItemRepository.Value;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
