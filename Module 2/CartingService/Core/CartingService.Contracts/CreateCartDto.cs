using System;
using System.ComponentModel.DataAnnotations;

namespace CartingService.Contracts
{
    public class CreateCartDto
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
    }
}
