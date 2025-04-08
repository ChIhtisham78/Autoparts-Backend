using AutoMapper;
using Autopart.Application.Models;
using Autopart.Application.Models.Dto;
using Autopart.Application.Models.Products;
using Autopart.Application.Options;
using Autopart.Domain.Interfaces;


namespace Autopart.Application
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Autopart.Domain.Models.AspNetUser, UserDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.AspNetUser, StoreOwnerDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.AspNetUser, AddUserResponse>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Address, AddressDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.SubCategoryList, SubCategoryListDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.SubCategoryList, AddSubCategoryListDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Profile, ProfileDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Engine, EngineDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Engine, AddEngineDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.ManufacturerModel, ManufactureModelDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.ManufacturerModel, AddManufactureModelDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Engine, EngineIdAndNameDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Image, ImageDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Billing, BillingsDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.BillingAddress, BillingsAddressDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Social, SocialDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Manufacture, ManufacturersDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Svcrelation, SVCRelationDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Type, TypeDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Banner, BannerDto>().ReverseMap();
			//CreateMap<AddUserDto,Autopart.Domain.Models.AspNetUser>();
			//CreateMap<AddUserResponse, Autopart.Domain.Models.AspNetUser>();

			CreateMap<Autopart.Domain.Models.Product, ProductDto>().ReverseMap();
            CreateMap<Autopart.Domain.Models.HomePage, HomePageDto>().ReverseMap();

            CreateMap<Autopart.Domain.Models.Shipping, ShippingsDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Product, ProductDtoResponse>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Tax, TaxDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.ShippingAddress, ShippingAddressDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Category, CategoryDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Image, ImageDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Gallery, ProductGalleryImageDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Category, ProductCategoryDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Tag, ProductTagDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Type, TypeDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Author, ProductAuthorDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Shop, ShopDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Address, ShopAddressDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Setting, SettingDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Balance, BalanceDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.OrderStatus, OrderStatusDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.OrderStatus, OrderStatusResponse>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Order, OrdersDto>().ReverseMap();

			CreateMap<Autopart.Domain.Models.Tag, TagDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Attribute, AttributeDto>()
				.ForMember(dest => dest.Values, opt => opt.MapFrom(src => src.Values))
				.ReverseMap();
			CreateMap<Autopart.Domain.Models.Value, ValueDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Coupon, CouponDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Author, AuthorDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Image, ProductImageDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.OrderLine, OrderLineForOrdersAdditionDto>().ReverseMap();
			CreateMap<LookupDto, LookupDto>().ReverseMap();
			CreateMap<StripeSetting, StripeSetting>().ReverseMap();
			CreateMap<Autopart.Domain.Models.OrderLine, OrdersProductResponse>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Order, OrdersDto>()
				.ForMember(dest => dest.OrderLines, opt => opt.MapFrom(src => src.OrderLines))
				.ReverseMap();
			CreateMap<Autopart.Domain.Models.Order, AddOrderDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.PaymentHistory, PaymentHistoryDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Category, CategorySummeryDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Shop, ShopSummeryDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Product, ProductCountDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.AspNetUser, AddNewUserDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Shop, ShopResponseOrder>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Rating, ReviewDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Rating, GetReviewsDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Product, RatingProductsDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.AspNetUser, RatingUsersDto>().ReverseMap();
			CreateMap<Autopart.Domain.Models.Question, QuestionDto>().ReverseMap();
		}
	}
}
