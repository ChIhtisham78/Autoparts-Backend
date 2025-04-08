using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface ISVCRelationService
    {
        Task<IEnumerable<SVCRelationDto>> GetSVCRelationAsync(int? shopId = null, string? size = null);
        Task<SVCRelationDto> CreateSVCRelationAsync(SVCRelationDto dto);
        Task<SVCRelationDto> UpdateSVCRelationAsync(SVCRelationDto dto);
    }
}
