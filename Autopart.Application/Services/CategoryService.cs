using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ITypeAdapter _typeAdapter;

		public CategoryService(ICategoryRepository categoryRepository, ITypeAdapter typeAdapter)
		{
			_categoryRepository = categoryRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
		{
			try
			{
				if (categoryDto.ImageDto != null)
				{
					var image = new Domain.Models.Image
					{
						OriginalUrl = categoryDto.ImageDto.OriginalUrl,
						ThumbnailUrl = categoryDto.ImageDto.ThumbnailUrl,
						CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
						UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
					};

					var createdImage = await _categoryRepository.CreateImageAsync(image);
					categoryDto.ImageDto.Id = createdImage.Id;
				}

				var slug = await EnsureUniqueSlugAsync(categoryDto.Name, categoryDto.Slug);
				var category = new Category
				{
					Name = categoryDto.Name,
					Slug = slug,
					Size = categoryDto.Size,
					Language = categoryDto.Language,
					Icon = categoryDto.Icon,
					ImageId = categoryDto.ImageDto?.Id
				};

				var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
				var createdCategoryDto = _typeAdapter.Adapt<CategoryDto>(createdCategory);
				return createdCategoryDto;
			}
			catch (Exception ex)
			{
				throw;
			}
		}



		public async Task<CategoryDto> GetCategoryByIdAsync(int id)
		{
			var category = await _categoryRepository.GetCategoryByIdAsync(id);
			return _typeAdapter.Adapt<CategoryDto>(category);
		}

		public async Task<IEnumerable<CategoryDto>> GetCategoryBySlugAsync(string slug)
		{
			try
			{
				var category = await _categoryRepository.GetCategoryBySlug(slug);
				return _typeAdapter.Adapt<IEnumerable<CategoryDto>>(category);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<(IEnumerable<CategoryDto> Categories, int TotalCount)> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10)
		{
			var (categories, totalCount) = await _categoryRepository.GetAllCategoriesAsync(pageNumber, pageSize);

			var categoryDtos = categories.Select(category => new CategoryDto
			{
				Id = category.Id,
				Name = category.Name,
				Slug = category.Slug,
				Language = category.Language,
				Icon = category.Icon,
				Size = category.Size,
				ParentId = category.ParentId,
				CreatedAt = category.CreatedAt,
				UpdatedAt = category.UpdatedAt,
				DeletedAt = category.DeletedAt,
				TypeId = category.TypeId,
				ImageDto = category.ImageId.HasValue && category.Image != null
					? new ImageDto
					{
						Id = category.Image.Id,
						OriginalUrl = category.Image.OriginalUrl,
						ThumbnailUrl = category.Image.ThumbnailUrl,
						CreatedAt = category.Image.CreatedAt,
						UpdatedAt = category.Image.UpdatedAt
					} : null ?? new ImageDto()
			});

			return (categoryDtos, totalCount);
		}


		public async Task<IEnumerable<CategoryDto>> GetCategoriesByParamAsync(string param, string language)
		{
			var categories = await _categoryRepository.GetCategoriesByParamAsync(param, language);
			return _typeAdapter.Adapt<IEnumerable<CategoryDto>>(categories);
		}

		public async Task<IEnumerable<CategoryDto>> GetCategoriesByName(string param)
		{
			var categories = await _categoryRepository.GetCategoriesByNameAsync(param);
			return _typeAdapter.Adapt<IEnumerable<CategoryDto>>(categories);
		}




		public async Task UpdateCategoryAsync(CategoryDto categoryDto)
		{
			try
			{
				var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryDto.Id);

				if (existingCategory == null)
				{
					throw new KeyNotFoundException("Category not found.");
				}
				existingCategory.Name = categoryDto.Name;
				existingCategory.Slug = categoryDto.Slug;
				existingCategory.Size = categoryDto.Size;
				existingCategory.Language = categoryDto.Language;
				existingCategory.Icon = categoryDto.Icon;
				existingCategory.TypeId = null;
				existingCategory.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
				existingCategory.ParentId = null;

				if (categoryDto.ImageDto != null && existingCategory.ImageId.HasValue)
				{
					var existingImage = await _categoryRepository.GetImageByIdAsync(existingCategory.ImageId.Value);
					existingImage.OriginalUrl = categoryDto.ImageDto.OriginalUrl;
					existingImage.ThumbnailUrl = categoryDto.ImageDto.ThumbnailUrl;
					existingImage.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

					await _categoryRepository.UpdateImageAsync(existingImage);
				}

				await _categoryRepository.UpdateCategoryAsync(existingCategory);
			}
			catch (Exception ex)
			{
				throw;
			}
		}


		public async Task DeleteCategoryAsync(int id)
		{
			var category = await _categoryRepository.GetCategoryByIdAsync(id);
			if (category == null)
			{
				throw new KeyNotFoundException("Category not found.");
			}

			if (category.ImageId.HasValue)
			{
				await _categoryRepository.DeleteImageAsync(category.ImageId.Value);
			}

			await _categoryRepository.DeleteCategoryAsync(id);
		}


		private string GenerateSlug(string name)
		{
			return name.ToLower().Replace(" ", "-");
		}

		private async Task<string> EnsureUniqueSlugAsync(string name, string requestedSlug)
		{
			string slug = GenerateSlug(requestedSlug);

			if (!await _categoryRepository.SlugExistsAsync(slug))
			{
				return slug;
			}

			int counter = 1;
			string originalSlug = slug;

			while (await _categoryRepository.SlugExistsAsync(slug))
			{
				slug = $"{originalSlug}-{counter}";
				counter++;
			}

			return slug;
		}
	}
}
