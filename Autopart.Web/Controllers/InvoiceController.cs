using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Dto;
using Autopart.Data;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IShippingsRepository _shippingsRepository;
        private readonly autopartContext _context;
        private readonly IOrdersService _ordersService;

        public InvoiceController(IOrdersService ordersService, IOrdersRepository ordersRepository, autopartContext context, IShippingsRepository shippingsRepository)
        {
            _ordersService = ordersService;
            _context = context;
            _shippingsRepository = shippingsRepository;
            _ordersRepository = ordersRepository;
        }

        [HttpPost("download-invoice-url")]
        public async Task<IActionResult> DownloadInvoiceUrl([FromBody] InvoiceRequest request)
        {
            var order = await _ordersService.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            var pdfBytes = await GenerateSimplePdf(order, request);
            return File(pdfBytes, "application/pdf", $"Invoice_{order.Id}.pdf");
        }


        private async Task<byte[]> GenerateSimplePdf(Order order, InvoiceRequest request)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Fetch customer data
                var customerDto = await FetchCustomerDataAsync(order.CustomerId);

                // Add document sections
                AddInvoiceHeader(document, order);
                AddCompanyAndCustomerInfo(document, customerDto);
                AddProductsTable(document, order);

                // Get the shipping address
                var shippingAddress = order.ShippingAddresses.FirstOrDefault();

                // Pass the shipping address to AddSummarySection
                await AddSummarySection(document, order, shippingAddress);

                document.Close();
                return memoryStream.ToArray();
            }
        }



        private async Task<CustomerDto> FetchCustomerDataAsync(int? customerId)
        {
            if (!customerId.HasValue)
            {
                throw new ArgumentException("OrderId cannot null");
            }

            var user = await _ordersRepository.GetOrderByCustomerId(customerId!.Value);
            var userAddress = await _ordersRepository.GetAddressByCustomerId(customerId.Value);

            return new CustomerDto
            {
                UserName = user!.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AddressDto = new AddressDto
                {
                    StreetAddress = userAddress!.StreetAddress,
                    City = userAddress.City,
                    State = userAddress.State,
                    Country = userAddress.Country,
                    Zip = userAddress.Zip,
                    UserId = userAddress.UserId
                }
            };
        }

        private void AddCompanyAndCustomerInfo(Document document, CustomerDto customerDto)
        {
            var tableHeader = new Table(2).UseAllAvailableWidth();
            tableHeader.SetMarginBottom(20);

            var companyInfo = new Paragraph()
                .Add(new Text("AutoParts").SetBold())
                .Add("\nhttps://www.easypartshub.com/")
                .Add("\n+129290122122")
                .Add("\nNY State Thruway, New York, USA")
                .SetTextAlignment(TextAlignment.RIGHT);

            var customerInfo = new Paragraph()
                .Add(new Text("Customer").SetBold())
                .Add($"\n{customerDto.UserName}")
                .Add($"\n{customerDto.Email}")
                .Add($"\n{customerDto.PhoneNumber}")
                .Add($"\n{customerDto.AddressDto?.StreetAddress}")
                .Add($"\n{customerDto.AddressDto?.City}")
                .Add($"\n{customerDto.AddressDto?.State} {customerDto.AddressDto?.Zip}")
                .Add($"\n{customerDto.AddressDto?.Country}")
                .SetTextAlignment(TextAlignment.LEFT);

            tableHeader.AddCell(new Cell().Add(customerInfo).SetBorder(Border.NO_BORDER));
            tableHeader.AddCell(new Cell().Add(companyInfo).SetBorder(Border.NO_BORDER));

            document.Add(tableHeader);
        }

        private void AddInvoiceHeader(Document document, Order order)
        {
            var invoiceHeaderTable = new Table(2).UseAllAvailableWidth();
            invoiceHeaderTable.SetMarginTop(20);

            invoiceHeaderTable.AddCell(new Cell().Add(new Paragraph($"Invoice No: {order.Id}").SetTextAlignment(TextAlignment.LEFT)).SetBorder(Border.NO_BORDER));
            invoiceHeaderTable.AddCell(new Cell().Add(new Paragraph($"Date: {DateTime.Now:dd MMM, yyyy}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            document.Add(invoiceHeaderTable);
        }


        private void AddProductsTable(Document document, Order order)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 50, 25, 25 })).UseAllAvailableWidth();
            var darkGreen = new iText.Kernel.Colors.DeviceRgb(0, 110, 0);
            var whiteColor = iText.Kernel.Colors.ColorConstants.WHITE;

            table.AddHeaderCell(new Cell().Add(new Paragraph("Products").SetFontColor(whiteColor)).SetBackgroundColor(darkGreen));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity").SetFontColor(whiteColor)).SetBackgroundColor(darkGreen));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFontColor(whiteColor)).SetBackgroundColor(darkGreen));

            foreach (var line in order.OrderLines)
            {
                var productName = line.Product?.Name;
                var quantity = line.Quantity?.ToString();
                var total = line.Total?.ToString("C");

                table.AddCell(productName);
                table.AddCell(quantity);
                table.AddCell(total);
            }

            document.Add(table);
        }

        private async Task AddSummarySection(Document document, Order order, ShippingAddress? shippingAddress)
        {
            decimal subtotal = 0m;
            decimal totalShippingCharges = 0m;

            foreach (var orderLine in order.OrderLines)
            {
                var product = orderLine.Product;
                var category = product?.CategoryId.HasValue ?? false
                    ? await _context.Categories.FindAsync(product.CategoryId.Value)
                    : null;

                var shippingCharges = (category != null && product!.ShopId.HasValue)
                    ? (await _shippingsRepository.GetSVCRelationByShopIdAndSizeAsync(product.ShopId.Value, category.Size))?.Price ?? 0
                    : 0;

                var totalPrice = (product?.Price ?? 0) * (orderLine.Quantity ?? 0);
                subtotal += totalPrice;

                totalShippingCharges += shippingCharges * (orderLine.Quantity ?? 0);
            }

            var totalDiscount = order.OrderLines.Sum(line => line.Discount ?? 0);

            decimal totalTax = 0;
            if (shippingAddress != null)
            {
                var taxRate = await _shippingsRepository.GetTaxRateForUserAddressAsync(shippingAddress);
                totalTax = taxRate * subtotal;
            }

            var totalAmount = subtotal - totalDiscount + totalTax + totalShippingCharges;

            var summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 })).UseAllAvailableWidth();

            summaryTable.AddCell(new Cell().Add(new Paragraph("SubTotal:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            summaryTable.AddCell(new Cell().Add(new Paragraph($"{subtotal:C}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

            summaryTable.AddCell(new Cell().Add(new Paragraph("Discount:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            summaryTable.AddCell(new Cell().Add(new Paragraph($"{totalDiscount:C}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

            summaryTable.AddCell(new Cell().Add(new Paragraph("Tax:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            summaryTable.AddCell(new Cell().Add(new Paragraph($"{totalTax:C}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

            summaryTable.AddCell(new Cell().Add(new Paragraph("Shipping Charges:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            summaryTable.AddCell(new Cell().Add(new Paragraph($"{totalShippingCharges:C}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));

            summaryTable.AddCell(new Cell().Add(new Paragraph("Paid Total:").SetTextAlignment(TextAlignment.RIGHT).SetBold()).SetBorder(Border.NO_BORDER));
            summaryTable.AddCell(new Cell().Add(new Paragraph($"{totalAmount:C}").SetTextAlignment(TextAlignment.RIGHT).SetBold()).SetBorder(Border.NO_BORDER));

            document.Add(summaryTable);
        }
    }
}
