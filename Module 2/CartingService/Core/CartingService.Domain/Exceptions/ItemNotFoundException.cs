namespace CartingService.Domain.Exceptions
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int itemId)
            : base($"The item with the identifier {itemId} was not found.")
        {
        }
    }
}
