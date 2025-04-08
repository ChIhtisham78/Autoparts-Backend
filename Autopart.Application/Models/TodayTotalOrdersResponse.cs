namespace Autopart.Application.Models
{
    public class TodayTotalOrdersResponse
    {
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Complete { get; set; }
        public int Cancelled { get; set; }
        public int Refunded { get; set; }
        public int Failed { get; set; }
        public int LocalFacility { get; set; }
        public int OutForDelivery { get; set; }
    }
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
