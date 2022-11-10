namespace CatringService.Services.Abstractions
{
    public interface IServiceManager
    {
        ICartService CartService { get; }

        IItemService ItemService { get; }
    }
}
