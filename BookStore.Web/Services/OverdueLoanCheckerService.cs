using BookStore.Service.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookStore.Web.Services
{
    public class OverdueLoanCheckerService : BackgroundService
    {
        private readonly ILogger<OverdueLoanCheckerService> _logger;
        private readonly IServiceProvider _services;

        public OverdueLoanCheckerService(
            ILogger<OverdueLoanCheckerService> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Gecikmiş Kredi Kontrol Servisi çalışıyor.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();
                        await loanService.CheckOverdueLoansAsync();
                    }

                    _logger.LogInformation("Gecikmiş krediler kontrol edildi: {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gecikmiş krediler kontrol edilirken hata oluştu");
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
