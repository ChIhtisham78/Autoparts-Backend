using Autopart.Application.Models.Products;

namespace Autopart.Application.Interfaces
{
    public interface IShippingsService
    {
        Task<ShippingsDto> AddShippings(ShippingsDto shippingsDto);
        Task<List<ShippingsDto>> GetShippings();
        Task<ShippingsDto> GetShippingsById(int id);
        Task<ShippingsDto> UpdateShippings(int id, ShippingsDto shippingsDto);
        Task<bool> RemoveShippings(int id);
    }
}
