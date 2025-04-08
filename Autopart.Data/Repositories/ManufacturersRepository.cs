using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class ManufacturersRepository : IManufacturersRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public ManufacturersRepository(autopartContext context)
		{
			_context = context;
		}


		public async Task<Manufacture> GetBySlugAsync(string slug)
		{
			return await _context.Manufactures.FirstOrDefaultAsync(m => m.Slug == slug);
		}

		public async Task<(List<Manufacture> Manufacturers, int TotalCount)> GetManufacturers(int pageNumber = 1, int pageSize = 10)
		{
			var query = _context.Manufactures.AsQueryable();
			var totalCount = await query.CountAsync();

			var manufacturers = await query
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (manufacturers, totalCount);
		}




		public async Task<Manufacture?> GetManufacturerById(int id)
		{
			return await _context.Manufactures.FindAsync(id);
		}

		public async Task<Manufacture?> GetManufacturerByName(string name)
		{
			return await _context.Manufactures.SingleOrDefaultAsync(m => m.Name == name);
		}
		public async Task<Manufacture?> GetManufacturerBySlug(string slug)
		{
			return await _context.Manufactures.FindAsync(slug);
		}

		public async Task<Image?> GetImageById(int id)
		{
			return await _context.Images.FindAsync(id);
		}

		public async Task<Social?> GetSocialById(int id)
		{
			return await _context.Socials.FindAsync(id);
		}

		public async Task<Domain.Models.Type?> GetTypeById(int id)
		{
			return await _context.Types.FindAsync(id);
		}

		public async Task<Banner?> GetBannerById(int id)
		{
			return await _context.Banners.FindAsync(id);
		}

		public async Task<PromotionalSlider?> GetPromotionalsliderById(int id)
		{
			return await _context.PromotionalSliders.FindAsync(id);
		}

		public void AddManufacturers(Manufacture manufacture)
		{
			_context.Manufactures.Add(manufacture);
		}

		public void AddSocial(Social social)
		{
			_context.Socials.Add(social);
		}

		public void AddImage(Image image)
		{
			_context.Images.Add(image);
		}

		public void AddType(Domain.Models.Type type)
		{
			_context.Types.Add(type);
		}
		public void AddBanner(Banner banner)
		{
			_context.Banners.Add(banner);
		}
		public void AddPromotionalslider(PromotionalSlider promotionalSlider)
		{
			_context.PromotionalSliders.Add(promotionalSlider);
		}

		public void UpdateManufacturers(Manufacture manufacture)
		{
			_context.Manufactures.Update(manufacture);
		}

		public void UpdateType(Domain.Models.Type type)
		{
			_context.Types.Update(type);
		}

		public void UpdateSocial(Social social)
		{
			_context.Socials.Update(social);
		}

		public void UpdateImage(Image image)
		{
			_context.Images.Update(image);
		}

		public void UpdateBanner(Banner banner)
		{
			_context.Banners.Update(banner);
		}

		public void UpdatePromotionalslider(PromotionalSlider promotionalSlider)
		{
			_context.PromotionalSliders.Update(promotionalSlider);
		}

		public void DeleteManufacturers(Manufacture manufacture)
		{
			_context.Manufactures.Remove(manufacture);
		}

		public async Task<bool> SlugExistsAsync(string slug)
		{
			return await _context.Manufactures.AnyAsync(p => p.Slug == slug);
		}
	}
}
