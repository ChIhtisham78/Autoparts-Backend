using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ISVCRelationRepository : IRepository<Svcrelation>
    {
        Task<IEnumerable<Svcrelation>> GetSVCRelationAsync(int? shopId = null, string? size = null);
        Task AddSVCRelationsAsync(Svcrelation svcRelations);
        void UpdateSVCRelation(Svcrelation svcRelation);
        Task<Svcrelation> GetSVCRelationByIdAsync(int id);


    }

}