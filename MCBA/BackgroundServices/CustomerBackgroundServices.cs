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
			await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken); // Check every minute
		}
	}

	private async Task CheckScheduledPayments(CancellationToken cancellationToken)
	{
		using var scope = _services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<MCBAContext>();

		var paymentsDue = dbContext.BillPays
			.Include(bp => bp.Account)
			.Where(bp => !bp.IsFailed && (bp.ScheduleTimeUtc <= DateTime.UtcNow))
			.ToList();

		foreach (var payment in paymentsDue)
		{
			// Check account balance and process payment
			if (payment.Account.Balance >= payment.Amount)
			{
				payment.Account.Balance -= payment.Amount;
				// Further processing like transaction logging
			}
			else
			{
				payment.IsFailed = true;
			}

			// Handle periodic payments
			if (payment.Period == 'M')
			{
				payment.ScheduleTimeUtc = payment.ScheduleTimeUtc.AddMonths(1);
			}
			else if (payment.Period == 'O')
			{
				// Handle one-off payment logic, if needed
			}

			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
