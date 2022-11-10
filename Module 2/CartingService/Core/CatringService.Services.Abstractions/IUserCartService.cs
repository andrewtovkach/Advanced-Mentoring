using System;

namespace CatringService.Services.Abstractions
{
    public interface IUserCartService
    {
        Guid GetCurrentCartIdentifier();
    }
}
