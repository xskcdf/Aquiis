using Microsoft.Extensions.Options;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Constants;

namespace Aquiis.SimpleStart.Application.Services
{
    public class ApplicationService
    {
        private readonly ApplicationSettings _settings;
        private readonly PaymentService _paymentService;
        private readonly LeaseService _leaseService;
        
        public bool SoftDeleteEnabled { get; }

        public ApplicationService(
            IOptions<ApplicationSettings> settings,
            PaymentService paymentService,
            LeaseService leaseService)
        {
            _settings = settings.Value;
            _paymentService = paymentService;
            _leaseService = leaseService;
            SoftDeleteEnabled = _settings.SoftDeleteEnabled;
        }

        public string GetAppInfo()
        {
            return $"{_settings.AppName} - {_settings.Version}";
        }

        /// <summary>
        /// Gets the total payments received for a specific date
        /// </summary>
        public async Task<decimal> GetDailyPaymentTotalAsync(DateTime date)
        {
            var payments = await _paymentService.GetAllAsync();
            return payments
                .Where(p => p.PaidOn.Date == date.Date && !p.IsDeleted)
                .Sum(p => p.Amount);
        }

        /// <summary>
        /// Gets the total payments received for today
        /// </summary>
        public async Task<decimal> GetTodayPaymentTotalAsync()
        {
            return await GetDailyPaymentTotalAsync(DateTime.Today);
        }

        /// <summary>
        /// Gets the total payments received for a date range
        /// </summary>
        public async Task<decimal> GetPaymentTotalForRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentService.GetAllAsync();
            return payments
                .Where(p => p.PaidOn.Date >= startDate.Date && 
                           p.PaidOn.Date <= endDate.Date && 
                           !p.IsDeleted)
                .Sum(p => p.Amount);
        }

        /// <summary>
        /// Gets payment statistics for a specific period
        /// </summary>
        public async Task<PaymentStatistics> GetPaymentStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentService.GetAllAsync();
            var periodPayments = payments
                .Where(p => p.PaidOn.Date >= startDate.Date && 
                           p.PaidOn.Date <= endDate.Date && 
                           !p.IsDeleted)
                .ToList();

            return new PaymentStatistics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalAmount = periodPayments.Sum(p => p.Amount),
                PaymentCount = periodPayments.Count,
                AveragePayment = periodPayments.Any() ? periodPayments.Average(p => p.Amount) : 0,
                PaymentsByMethod = periodPayments
                    .GroupBy(p => p.PaymentMethod)
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount))
            };
        }

        /// <summary>
        /// Gets leases expiring within the specified number of days
        /// </summary>
        public async Task<int> GetLeasesExpiringCountAsync(int daysAhead)
        {
            var leases = await _leaseService.GetAllAsync();
            return leases
                .Where(l => l.EndDate >= DateTime.Today && 
                           l.EndDate <= DateTime.Today.AddDays(daysAhead) && 
                           !l.IsDeleted)
                .Count();
        }
    }

    public class PaymentStatistics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentCount { get; set; }
        public decimal AveragePayment { get; set; }
        public Dictionary<string, decimal> PaymentsByMethod { get; set; } = new();
    }
}