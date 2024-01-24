using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MCBA.Models;
using MCBA.Data;

namespace MCBA.CustomerBackgroundServices;

public class CustomerBackgroundServices : BackgroundService
{
	private readonly IServiceProvider _services;

	public CustomerBackgroundServices(IServiceProvider services)
	{
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			await CheckScheduledPayments(cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }

    private async Task CheckScheduledPayments(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MCBAContext>();

        try
        {
            var allPayments = await dbContext.BillPays
                .Include(bp => bp.Account)
                .Where(bp => !bp.IsFailed && !bp.Processed)
                .ToListAsync(cancellationToken);

            Console.WriteLine($"Found {allPayments.Count} payments to check.");

            foreach (var payment in allPayments)
            {
                if (payment.ScheduleTimeUtc > DateTime.Now)
                {
                    continue; // Skip to the next payment if not due
                }

                Console.WriteLine($"Processing payment {payment.BillPayID} for account {payment.AccountNumber}.");

                // Check account balance and process payment
                if (payment.Account.Balance >= payment.Amount)
                {
                    payment.Account.Balance -= payment.Amount;
                }
                else
                {
                    payment.IsFailed = true;
                }

                // Handle periodic payments
                if (payment.Period == 'M')
                {
                    payment.ScheduleTimeUtc = payment.ScheduleTimeUtc.AddMonths(1);

                    // Billpay yet to be processed again
                    payment.Processed = false;
                }
                else if (payment.Period == 'O')
                {
                    // BillPay finished Processeing
                    payment.Processed = true;

                }

                dbContext.BillPays.Update(payment);

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckScheduledPayments: {ex.Message}");
        }
    }


}
