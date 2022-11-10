using CartingService.Contracts;
using CartingService.Domain.Exceptions;
using CartingService.Domain.Repositories;
using CatringService.Services.Abstractions;
using System;
using System.Collections.Generic;
using CartingService.Domain.Entities;
using AutoMapper;

namespace CartingService.Services
{
    public class ItemService : IItemService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public ItemService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public IEnumerable<ItemDto> GetAllByCartId(Guid cartId)
        {
            var items = _repositoryManager.ItemRepository.GetAllByCartId(cartId);

            var itemsDto = _mapper.Map<IEnumerable<Item>, List<ItemDto>>(items);

            return itemsDto;
        }

        public ItemDto GetById(Guid cartId, int itemId)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            var item = _repositoryManager.ItemRepository.GetById(itemId);

            if (item is null)
            {
                throw new ItemNotFoundException(itemId);
            }

            if (item.CartId != cart.Id)
            {
                throw new ItemDoesNotBelongToCartException(cart.Id, item.Id);
            }

            var itemDto = _mapper.Map<Item, ItemDto>(item);

            return itemDto;
        }

        public ItemDto Create(Guid cartId, CreateItemDto itemForCreationDto)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            var item = _mapper.Map<CreateItemDto, Item>(itemForCreationDto);

            item.CartId = cart.Id;

            _repositoryManager.ItemRepository.Insert(item);

            return _mapper.Map<Item, ItemDto>(item);
        }

        public void Update(Guid cartId, UpdateItemDto itemForUpdateDto)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            var item = _mapper.Map<UpdateItemDto, Item>(itemForUpdateDto);

            item.CartId = cart.Id;

            _repositoryManager.ItemRepository.Update(item);
        }

        public void Delete(Guid cartId, int itemId)
        {
            var cart = _repositoryManager.CartRepository.GetById(cartId);

            if (cart is null)
            {
                throw new CartNotFoundException(cartId);
            }

            var item = _repositoryManager.ItemRepository.GetById(itemId);

            if (item is null)
            {
                throw new ItemNotFoundException(itemId);
            }

            if (item.CartId != cart.Id)
            {
                throw new ItemDoesNotBelongToCartException(cart.Id, item.Id);
            }

            _repositoryManager.ItemRepository.Remove(itemId);
        }
    }
}
