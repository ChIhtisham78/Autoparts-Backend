using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface IAttributeService
    {
        Task<AttributeDto> CreateAttributeAsync(AttributeDto attributeDto);
        Task<AttributeDto> GetAttributeByIdAsync(int id);
        Task<IEnumerable<AttributeDto>> GetAttributesAsync(int? shopId);

        Task<IEnumerable<AttributeDto>> GetAttributesByParamAsync(string param);
        Task UpdateAttributeAsync(AttributeDto attributeDto);
        Task RemoveAttributeAsync(int id);
    }
}
