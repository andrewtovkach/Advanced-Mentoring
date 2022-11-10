using System.ComponentModel.DataAnnotations;

namespace CartingService.Contracts
{
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be loner then 100 characters")]
        public string Name { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater than zero")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than zero")]
        public double Quantity { get; set; }
    }
}
