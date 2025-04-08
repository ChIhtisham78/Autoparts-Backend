using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
    public class SVCRelationService : ISVCRelationService
    {
        private readonly ISVCRelationRepository _svcRelationRepository;
        private readonly ITypeAdapter _typeAdapter;

        public SVCRelationService(ISVCRelationRepository svcRelationRepository, ITypeAdapter typeAdapter)
        {
            _svcRelationRepository = svcRelationRepository;
            _typeAdapter = typeAdapter;
        }

        public async Task<IEnumerable<SVCRelationDto>> GetSVCRelationAsync(int? shopId = null, string? size = null)
        {
            var svcRelations = await _svcRelationRepository.GetSVCRelationAsync(shopId, size);

            if (svcRelations == null || !svcRelations.Any())
            {
                return new List<SVCRelationDto>();
            }

            return _typeAdapter.Adapt<IEnumerable<SVCRelationDto>>(svcRelations);
        }

        public async Task<SVCRelationDto> CreateSVCRelationAsync(SVCRelationDto dto)
        {
            try
            {
                var svcRelation = new Svcrelation
                {
                    ShopId = dto.ShopId,
                    Size = dto.Size,
                    Price = dto.Price
                };

                await _svcRelationRepository.AddSVCRelationsAsync(svcRelation);
                await _svcRelationRepository.UnitOfWork.SaveChangesAsync();

                return _typeAdapter.Adapt<SVCRelationDto>(svcRelation);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<SVCRelationDto> UpdateSVCRelationAsync(SVCRelationDto dto)
        {
            var existingSVCRelation = await _svcRelationRepository.GetSVCRelationByIdAsync(dto.Id);
            if (existingSVCRelation == null)
            {
                return null;
            }
            existingSVCRelation.Price = dto.Price;
            existingSVCRelation.ShopId = dto.ShopId;
            existingSVCRelation.Size = dto.Size;

            _svcRelationRepository.UpdateSVCRelation(existingSVCRelation);
            await _svcRelationRepository.UnitOfWork.SaveChangesAsync();

            return _typeAdapter.Adapt<SVCRelationDto>(existingSVCRelation);
        }


    }
}
