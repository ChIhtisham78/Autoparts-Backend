using Autopart.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<int> GetTotalShopsCount(int? vendorId = null);
        Task<AnalyticsSummaryResponse> GetAnalyticsSummary(int? vendorId = null);
    }
}
