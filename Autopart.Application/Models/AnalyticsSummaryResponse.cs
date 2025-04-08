using Autopart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class AnalyticsSummaryResponse
    {
        public decimal TotalRevenue {  get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalShops { get; set; }
        public int TotalVendors { get; set; }
        public int TotalOrders { get; set; }
        public int TotalRefunds {  get; set; }  
        public int NewCustomers { get; set; }
        //public List<NewlyCreatedUsers> NewlyCreatedUsers { get; set; }

        public TodayTotalOrdersResponse? TodayTotalOrderByStatus { get; set; }
        public WeeklyTotalOrdersByStatus? WeeklyTotalOrderByStatus { get; set; }
        public MonthlyTotalOrdersResponse? MonthlyTotalOrderByStatus { get; set; }
       public YearlyTotalOrdersByStatus? YearlyTotalOrderByStatus { get; set; }
        public List<TotalYearSaleByMonth>? TotalYearSaleByMonth { get; set; }
    }
}
