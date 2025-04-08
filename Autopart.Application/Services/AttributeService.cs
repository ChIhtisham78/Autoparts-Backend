using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
    public class AttributeService : IAttributeService
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly ITypeAdapter _typeAdapter;

        public AttributeService(IAttributeRepository attributeRepository, ITypeAdapter typeAdapter)
        {
            _attributeRepository = attributeRepository;
            _typeAdapter = typeAdapter;
        }

        public async Task<AttributeDto> CreateAttributeAsync(AttributeDto attributeDto)
        {
            try
            {
                var attribute = new Domain.Models.Attribute
                {
                    Name = attributeDto.Name,
                    Slug = attributeDto.Slug,
                    Language = attributeDto.Language,
                    UserId = attributeDto.UserId,
                    ShopId = attributeDto.ShopId
                };

                var createdAttribute = await _attributeRepository.CreateAttributeAsync(attribute);
                foreach (var valueDto in attributeDto.Values)
                {
                    var value = new Domain.Models.Value
                    {
                        Value1 = valueDto.Value1,
                        AttributeId = createdAttribute.Id,
                        Slug = valueDto.Slug,
                        Meta = valueDto.Meta,
                        Language = valueDto.Language
                    };
                    await _attributeRepository.CreateValueAsync(value);
                }

                var createdAttributeDto = _typeAdapter.Adapt<AttributeDto>(createdAttribute);
                return createdAttributeDto;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the attribute", ex);
            }
        }

        public async Task<AttributeDto> GetAttributeByIdAsync(int id)
        {
            var attribute = await _attributeRepository.GetAttributeByIdAsync(id);
            return _typeAdapter.Adapt<AttributeDto>(attribute);
        }



        public async Task<IEnumerable<AttributeDto>> GetAttributesAsync(int? shopId)
        {
            var attributes = await _attributeRepository.GetAttributesAsync(shopId);
            return _typeAdapter.Adapt<IEnumerable<AttributeDto>>(attributes);
        }




        public async Task<IEnumerable<AttributeDto>> GetAttributesByParamAsync(string param)
        {
            var attributes = await _attributeRepository.GetAttributeByParamAsync(param);
            return _typeAdapter.Adapt<IEnumerable<AttributeDto>>(attributes);
        }



        public async Task RemoveAttributeAsync(int id)
        {
            await _attributeRepository.RemoveValueByAttributeAsync(id);
            await _attributeRepository.RemoveAttributeAsync(id);
        }



        public async Task UpdateAttributeAsync(AttributeDto attributeDto)
        {
            try
            {
                var existingAttribute = await _attributeRepository.GetAttributeByIdAsync(attributeDto.Id);

                if (existingAttribute == null)
                {
                    throw new KeyNotFoundException("Attribute not found.");
                }

                // Update attribute properties
                existingAttribute.Name = attributeDto.Name;
                existingAttribute.Slug = attributeDto.Slug;
                existingAttribute.Language = attributeDto.Language;
                existingAttribute.ShopId = attributeDto.ShopId;

                // Track IDs of values to be updated or created
                var valueIdsToUpdate = new HashSet<int>();

                // Update or add values
                foreach (var valueDto in attributeDto.Values)
                {
                    // Check if value already exists in the attribute
                    var existingValue = existingAttribute.Values.FirstOrDefault(v => v.Id == valueDto.Id);

                    if (existingValue != null)
                    {
                        // Update existing value
                        existingValue.Value1 = valueDto.Value1;
                        existingValue.Slug = valueDto.Slug;
                        existingValue.Meta = valueDto.Meta;
                        existingValue.Language = valueDto.Language;

                        // Update value in database
                        await _attributeRepository.UpdateValueAsync(existingValue);
                    }
                    else
                    {
                        // Add new value
                        var newValue = new Domain.Models.Value
                        {
                            Value1 = valueDto.Value1,
                            Slug = valueDto.Slug,
                            Meta = valueDto.Meta,
                            Language = valueDto.Language,
                            AttributeId = attributeDto.Id // Assuming the attribute ID is known
                        };

                        // Create new value in the database
                        var createdValue = await _attributeRepository.CreateValueAsync(newValue);
                        // Track the ID of the newly created value
                        valueIdsToUpdate.Add(createdValue.Id);
                    }

                    // Track the ID of the value that is updated or added
                    valueIdsToUpdate.Add(valueDto.Id);
                }

                // Remove values that are not present in the updated attributeDto
                var valuesToRemove = existingAttribute.Values.Where(v => !valueIdsToUpdate.Contains(v.Id)).ToList();
                foreach (var valueToRemove in valuesToRemove)
                {
                    await _attributeRepository.RemoveValueAsync(valueToRemove.Id);
                }

                // Update attribute in the database
                await _attributeRepository.UpdateAttributeAsync(existingAttribute);
            }
            catch (Exception ex)
            {
                // Handle exception
                throw;
            }
        }


    }
}
