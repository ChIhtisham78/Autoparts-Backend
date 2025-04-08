namespace Autopart.Application.Models.Dto
{
    public class BalanceDto
    {
        public int? Id { get; set; }

        public int AdminCommissionRate { get; set; }

        public string Shop { get; set; }

        public int? ShopId { get; set; }

        public decimal? TotalEarnings { get; set; }

        public decimal? WithDrawnAmount { get; set; }

        public decimal? CurrentBalance { get; set; }

        public string AccountNumber { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountHolderEmail { get; set; }

        public string BankName { get; set; }

    }
}
