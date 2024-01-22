using MCBA.Models;

namespace MCBA.ViewModels;

public class DepositAndWithdrawViewModel
{
    public int AccountNumber { get; set; }
    public Account Account { get; set; }
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
}
