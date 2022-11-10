using AutoMapper;
using CartingService.Domain.Repositories;
using CatringService.Services.Abstractions;
using System;

namespace CartingService.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICartService> _lazyCartService;
        private readonly Lazy<IItemService> _lazyItemService;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _lazyCartService = new Lazy<ICartService>(() => new CartService(repositoryManager, mapper));
            _lazyItemService = new Lazy<IItemService>(() => new ItemService(repositoryManager, mapper));
        }

        public ICartService CartService => _lazyCartService.Value;

        public IItemService ItemService => _lazyItemService.Value;
    }
}
