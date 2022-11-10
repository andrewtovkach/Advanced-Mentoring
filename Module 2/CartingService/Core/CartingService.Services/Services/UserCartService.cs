using CartingService.Domain.Repositories;
using CatringService.Services.Abstractions;
using System;
using System.Linq;

namespace CartingService.Services
{
    public class UserCartService : IUserCartService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserCartService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public Guid GetCurrentCartIdentifier()
        {
            var carts = _repositoryManager.CartRepository.GetAll();

            var currentCart = carts.FirstOrDefault();

            if(currentCart == null)
            {
                return Guid.Empty;
            }

            return currentCart.Id;
        }
    }
}
