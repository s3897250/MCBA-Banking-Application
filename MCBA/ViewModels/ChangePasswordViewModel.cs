using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MCBA.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

    }
}
