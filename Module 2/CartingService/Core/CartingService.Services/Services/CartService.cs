using AutoMapper;
using CartingService.Contracts;
using CartingService.Domain.Entities;
using CartingService.Domain.Exceptions;
using CartingService.Domain.Repositories;
using CatringService.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.Services
{
    public class CartService : ICartService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public CartService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public IEnumerable<CartDto> GetAll()
        {
            var carts = _repositoryManager.CartRepository.GetAll();

            return carts.Select(x => new CartDto
            {
                Id = x.Id,
                Items = _mapper.Map<IEnumerable<Item>, List<ItemDto>>(_repositoryManager.ItemRepository.GetAllByCartId(x.Id)).ToList()
            });
        }

        public CartDto GetById(Guid cartId)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            return new CartDto
            {
                Id = cart.Id,
                Items = _mapper.Map<IEnumerable<Item>, List<ItemDto>>(_repositoryManager.ItemRepository.GetAllByCartId(cart.Id)).ToList()
            };
        }

        public CartDto Create(CreateCartDto cartForCreationDto)
        {
            var cart = _mapper.Map<CreateCartDto, Cart>(cartForCreationDto);

            _repositoryManager.CartRepository.Insert(cart);

            return _mapper.Map<Cart, CartDto>(cart);
        }

        public void Delete(Guid cartId)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            _repositoryManager.CartRepository.Remove(cartId);
        }
    }
}
