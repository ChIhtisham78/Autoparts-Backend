using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IAttributeRepository : IRepository<Models.Attribute>
    {
        Task<Models.Attribute> CreateAttributeAsync(Models.Attribute attribute);
        Task<Models.Value> CreateValueAsync(Models.Value value);
        Task<Models.Attribute> GetAttributeByIdAsync(int id);
        Task<IEnumerable<Domain.Models.Attribute>> GetAttributesAsync(int? shopId);

        Task<IEnumerable<Models.Attribute>> GetAttributeByParamAsync(string param);
        Task UpdateAttributeAsync(Models.Attribute attribute);
        Task UpdateValueAsync(Value value);
        Task RemoveAttributeAsync(int id);
        Task RemoveValueAsync(int id);
        Task RemoveValueByAttributeAsync(int id);
    }
}