using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IManufacturersRepository : IRepository<Manufacture>
    {
        Task<(List<Manufacture> Manufacturers, int TotalCount)> GetManufacturers(int pageNumber = 1, int pageSize = 10);
        Task<Manufacture?> GetManufacturerById(int id);
        Task<Manufacture?> GetManufacturerBySlug(string slug);
        Task<Manufacture> GetBySlugAsync(string slug);

        Task<Image?> GetImageById(int id);
        Task<Social?> GetSocialById(int id);
        Task<Models.Type?> GetTypeById(int id);
        Task<Banner?> GetBannerById(int id);
        Task<PromotionalSlider?> GetPromotionalsliderById(int id);
        Task<Manufacture?> GetManufacturerByName(string name);
        void AddManufacturers(Manufacture manufacture);
        void UpdateManufacturers(Manufacture manufacture);
        void UpdateSocial(Social social);
        void UpdateImage(Image image);
        void UpdateBanner(Banner banner);
        void UpdatePromotionalslider(PromotionalSlider promotionalSlider);
        void UpdateType(Models.Type type);
        void DeleteManufacturers(Manufacture manufacture);
        public void AddSocial(Social social);
        public void AddImage(Image image);
        public void AddType(Domain.Models.Type type);
        public void AddBanner(Banner banner);
        public void AddPromotionalslider(PromotionalSlider promotionalSlider);
        Task<bool> SlugExistsAsync(string slug);

    }
}
