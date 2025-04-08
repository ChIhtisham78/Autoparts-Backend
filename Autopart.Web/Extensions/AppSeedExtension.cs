using Autopart.Application.Helpers;
using Autopart.Domain.Interfaces;

namespace Autopart.Api.Extensions
{
	public static class AppSeedExtension
	{
		public static void Seed(this IServiceProvider services)
		{
			using (var scope = services.CreateScope())
			{

				var dataSeeder = scope.ServiceProvider.GetRequiredService<ISeedRepository>();
				var passwordHelper = scope.ServiceProvider.GetRequiredService<IPasswordHelper>();
				bool isActive = true;

				//seeding SuperAdmin
				string adminEmail = "admin@autopart.com";
				string adminPasswordSalt = passwordHelper.CreateSalt(40);
				var adminPassword = passwordHelper.CreatePasswordHash(adminPasswordSalt, "Admin@123");
				string adminRole = "super_admin";
				string adminUserName = "admin@autopart.com";
				dataSeeder.AddUser(adminEmail, adminUserName, adminPassword, adminPasswordSalt, adminRole, isActive);

				//seeding billingManager
				string billingManagerEmail = "billingmanager@autopart.com";
				string billingManagerPasswordSalt = passwordHelper.CreateSalt(40);
				var billingManagerPassword = passwordHelper.CreatePasswordHash(billingManagerPasswordSalt, "BillingManager@123");
				string billingManagerRole = "billing_manager";
				string billingManagerUserName = "billingmanager@autopart.com";
				dataSeeder.AddUser(billingManagerEmail, billingManagerUserName, billingManagerPassword, billingManagerPasswordSalt, billingManagerRole, isActive);

				//seeding billingManager
				string stockManagerEmail = "stockmanager@autopart.com";
				string stockManagerPasswordSalt = passwordHelper.CreateSalt(40);
				var stockManagerPassword = passwordHelper.CreatePasswordHash(billingManagerPasswordSalt, "StockManager@123");
				string stockManagerRole = "stock_manager";
				string stockManagerUserName = "stockmanager@autopart.com";
				dataSeeder.AddUser(stockManagerEmail, stockManagerUserName, stockManagerPassword, stockManagerPasswordSalt, stockManagerRole, isActive);

				//seeding Vendor
				string vendorEmail = "vendor@autopart.com";
				string vendorPasswordSalt = passwordHelper.CreateSalt(40);
				var vendorPassword = passwordHelper.CreatePasswordHash(vendorPasswordSalt, "Vendor@123");
				string vendorRole = "store_owner";
				string vendorUserName = "vendor@autopart.com";
				dataSeeder.AddUser(vendorEmail, vendorUserName, vendorPassword, vendorPasswordSalt, vendorRole, isActive);
				//seeding User
				string userEmail = "user@autopart.com";
				string userPasswordSalt = passwordHelper.CreateSalt(40);
				var userPassword = passwordHelper.CreatePasswordHash(userPasswordSalt, "User@123");
				string userRole = "customer";
				string userUserName = "user@autopart.com";
				dataSeeder.AddUser(userEmail, userUserName, userPassword, userPasswordSalt, userRole, isActive);


				var orderStatuses = new List<string>
				{
					"order-pending",
					"order-processing",
					"order-completed",
					"order-cancelled",
					"order-refunded",
					"order-failed",
					"order-at-local-facility",
					"order-out-for-delivery"
				};

				foreach (var status in orderStatuses)
				{
					dataSeeder.AddOrderStatus(status);
				}
			}

		}
	}
}
