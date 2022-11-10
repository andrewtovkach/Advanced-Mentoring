using System;
using System.Collections.Generic;

namespace CartingService.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }

        public ICollection<Item> Items { get; set; }
    }
}
