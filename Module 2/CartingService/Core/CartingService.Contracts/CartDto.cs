using System;
using System.Collections.Generic;

namespace CartingService.Contracts
{
    public class CartDto
    {
        public Guid Id { get; set; }

        public ICollection<ItemDto> Items { get; set; }
    }
}
