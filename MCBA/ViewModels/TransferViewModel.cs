using MCBA.Models;

namespace MCBA.ViewModels;

public class TransferViewModel
{
    public int AccountNumber { get; set; }
    public Account Account { get; set; }
    public int DestinationAccountNumber { get; set; } // Added property
    public decimal Amount { get; set; }
    public string? Comment { get; set; }

    public List<Account> GlobalAccounts { get; set; } // Ensure this is a property, not a field
}
