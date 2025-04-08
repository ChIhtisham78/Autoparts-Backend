using Autopart.Application.Helpers;
using Autopart.Application.Interfaces;
using Autopart.Application.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppServicesExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ITempService, TempService>();
            services.AddTransient<IPasswordHelper, PasswordHelper>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITaxService, TaxSevice>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<IManufacturersService, ManufacturersService>();
            services.AddTransient<IShippingsService, ShippingsService>();
            services.AddTransient<IEngineService, EngineService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISVCRelationService, SVCRelationService>();
            services.AddTransient<IFlashSaleService, FlashSaleService>();
            services.AddTransient<IStripeService, StripeService>();
            services.AddTransient<IManufactureModelService, ManufactureModelService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ISubCategoryListService, SubCategoryListService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IAttributeService, AttributeService>();
            services.AddTransient<IHomePageService, HomePageService>();
            services.AddTransient<ICouponService, CouponService>();
            services.AddTransient<IOrderStatusService, OrderStatusService>();
            services.AddTransient<IBillingsService, BillingsService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IShopService, ShopService>();
            services.AddTransient<IAuthorService, AuthorService>();
            services.AddTransient<SendGridSetting, SendGridSetting>();
            services.AddTransient<IAnalyticsService, AnalyticsService>();
            services.AddTransient<IRatingService, RatingService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddDistributedMemoryCache();
        }
    }
}
