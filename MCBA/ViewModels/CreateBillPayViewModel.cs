using MCBA.Models;

namespace MCBA.ViewModels;


public class CreateBillPayViewModel
{
    public int AccountNumber { get; set; }
    public int PayeeID { get; set; }
    public decimal Amount { get; set; }
    public DateTime ScheduleTimeUtc { get; set; }
    public char Period { get; set; }

    public List<Payee> Payees { get; set; }
}
