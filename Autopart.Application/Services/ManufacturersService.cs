using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class ManufacturersService : IManufacturersService
	{
		private readonly IManufacturersRepository _manufacturersRepository;
		private readonly ITypeAdapter _typeAdapter;

		public ManufacturersService(IManufacturersRepository manufacturersRepository, ITypeAdapter typeAdapter)
		{
			_manufacturersRepository = manufacturersRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<(List<ManufacturersDto> Manufacturers, int TotalCount)> GetManufacturers(int pageNumber = 1, int pageSize = 10)
		{
			var (manufacturers, totalCount) = await _manufacturersRepository.GetManufacturers(pageNumber, pageSize);
			var manufacturersDtos = new List<ManufacturersDto>();

			foreach (var manufacturer in manufacturers)
			{
				var manufacturerDto = _typeAdapter.Adapt<ManufacturersDto>(manufacturer);

				if (manufacturer.ImageId.HasValue)
				{
					var image = await _manufacturersRepository.GetImageById(manufacturer.ImageId.Value);
					if (image != null)
					{
						manufacturerDto.ImageOriginalUrl = image.OriginalUrl;
						manufacturerDto.ImageThumbnailUrl = image.ThumbnailUrl;
					}
				}

				manufacturersDtos.Add(manufacturerDto);
			}

			return (manufacturersDtos, totalCount);
		}





		public async Task<ManufacturersDto> GetManufacturerBySlugAsync(string slug)
		{
			var manufacturer = await _manufacturersRepository.GetBySlugAsync(slug);

			if (manufacturer == null)
			{
				return null;
			}
			return _typeAdapter.Adapt<ManufacturersDto>(manufacturer);
		}


		public async Task<ManufacturersDto> GetManufacturerById(int id)
		{
			var manufacturer = await _manufacturersRepository.GetManufacturerById(id);
			var manufacturerDto = _typeAdapter.Adapt<ManufacturersDto>(manufacturer);

			if (manufacturer.ImageId.HasValue)
			{
				var image = await _manufacturersRepository.GetImageById(manufacturer.ImageId.Value);
				if (image != null)
				{
					manufacturerDto.ImageOriginalUrl = image.OriginalUrl;
					manufacturerDto.ImageThumbnailUrl = image.ThumbnailUrl;
				}
			}

			return manufacturerDto;
		}


		public async Task RemoveManufacturer(int id)
		{
			var manufacturer = await _manufacturersRepository.GetManufacturerById(id);
			if (manufacturer == null)
			{
				throw new DomainException("Manufacturer  not exists");
			}
			_manufacturersRepository.DeleteManufacturers(manufacturer);
			await _manufacturersRepository.UnitOfWork.SaveChangesAsync();

		}

		public async Task<ManufacturersDto> AddManufacturers(ManufacturersDto manufacturersDto, ImageDto imageDto, SocialDto socialDto/*, TypeDto typeDto, BannerDto bannerDto*/)
		{
			var social = new Social
			{
				Type = socialDto.Type,
				Link = socialDto.Link,
				Icon = socialDto.Icon,
				Url = socialDto.Url
			};

			_manufacturersRepository.AddSocial(social);
			await _manufacturersRepository.UnitOfWork.SaveChangesAsync();
			var socialId = social.Id;

			var image = new Image
			{
				ThumbnailUrl = imageDto.ThumbnailUrl,
				OriginalUrl = imageDto.OriginalUrl,
				CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
				UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
			};

			_manufacturersRepository.AddImage(image);
			await _manufacturersRepository.UnitOfWork.SaveChangesAsync();
			var imageId = image.Id;

			//var slug = GenerateSlug(manufacturersDto.Name);
			var slug = await EnsureUniqueSlugAsync(manufacturersDto.Name, manufacturersDto.Slug);

			var manufacturer = new Manufacture();

			manufacturer.ImageId = imageId;
			manufacturer.SocialId = socialId;
			manufacturer.Name = manufacturersDto.Name;
			manufacturer.Slug = slug;
			manufacturer.Description = manufacturersDto.Description;
			manufacturer.IsApproved = true;
			manufacturer.ProductsCount = manufacturersDto.ProductsCount;
			manufacturer.Website = manufacturersDto.Website;
			manufacturer.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
			manufacturer.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);


			_manufacturersRepository.AddManufacturers(manufacturer);
			await _manufacturersRepository.UnitOfWork.SaveChangesAsync();

			var manufacturerDto = _typeAdapter.Adapt<ManufacturersDto>(manufacturersDto);

			return manufacturerDto;
		}

		public async Task UpdateManufacturers(ManufacturersDto manufacturersDto, SocialDto socialDto, ImageDto imageDto/*, TypeDto typeDto, BannerDto bannerDto*/)
		{

			try
			{
				Manufacture? manufacturer = await _manufacturersRepository.GetManufacturerById(manufacturersDto.Id);

				if (manufacturer == null)
				{
					throw new DomainException("Manufacturer not exists");
				}
				manufacturer.Name = manufacturersDto.Name;
				manufacturer.Slug = manufacturersDto.Slug;
				manufacturer.Description = manufacturersDto.Description;
				manufacturer.IsApproved = true;
				manufacturer.ProductsCount = manufacturersDto.ProductsCount;
				manufacturer.Website = manufacturersDto.Website;
				manufacturer.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
				manufacturer.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
				_manufacturersRepository.UpdateManufacturers(manufacturer);
				await _manufacturersRepository.UnitOfWork.SaveChangesAsync();


				var social = await _manufacturersRepository.GetSocialById((int)manufacturer.SocialId);
				if (social != null)
				{
					social.Type = socialDto.Type;
					social.Link = socialDto.Link;
					social.Icon = socialDto.Icon;
					social.Url = socialDto.Url;
					_manufacturersRepository.UpdateSocial(social);
				}
				else
				{
					social = new Social
					{
						Type = socialDto.Type,
						Link = socialDto.Link,
						Icon = socialDto.Icon,
						Url = socialDto.Url
					};
					_manufacturersRepository.AddSocial(social);
				}
				await _manufacturersRepository.UnitOfWork.SaveChangesAsync();



				var image = await _manufacturersRepository.GetImageById((int)manufacturer.ImageId);
				if (image != null)
				{
					image.ThumbnailUrl = imageDto.ThumbnailUrl;
					image.OriginalUrl = imageDto.OriginalUrl;
					image.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
					image.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
					_manufacturersRepository.UpdateImage(image);
				}
				else
				{
					image = new Image
					{
						ThumbnailUrl = imageDto.ThumbnailUrl,
						OriginalUrl = imageDto.OriginalUrl,
						CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
						UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
					};
					_manufacturersRepository.AddImage(image);
				}

				_manufacturersRepository.UpdateImage(image);
				await _manufacturersRepository.UnitOfWork.SaveChangesAsync();

			}
			catch (Exception ex)
			{

				throw;
			}
		}

		private string GenerateSlug(string name)
		{
			return name.ToLower().Replace(" ", "-");
		}

		private async Task<string> EnsureUniqueSlugAsync(string name, string requestedSlug)
		{
			// Generate the initial slug from the requested name
			string slug = GenerateSlug(requestedSlug);

			// If the requested slug is not found in the database, return it directly
			if (!await _manufacturersRepository.SlugExistsAsync(slug))
			{
				return slug;
			}

			// Otherwise, append a counter until a unique slug is found
			int counter = 1;
			string originalSlug = slug;

			while (await _manufacturersRepository.SlugExistsAsync(slug))
			{
				slug = $"{originalSlug}-{counter}";
				counter++;
			}

			return slug;
		}

	}
}
