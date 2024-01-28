using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAdminPortal.Models
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string? Address { get; set; }

        [StringLength(40)]
        public string? City { get; set; }

        [StringLength(4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Must be 4 digits")]
        public string? PostCode { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^\d{3}\s\d{3}\s\d{3}$", ErrorMessage = "TFN must be of the format: XXX XXX XXX")]
        public string? TFN { get; set; }

        [StringLength(12)]
        [RegularExpression(@"^04\d{2}\s\d{3}\s\d{3}$", ErrorMessage = "Mobile must be of the format: 04XX XXX XXX")]
        public string? Mobile { get; set; }

        [StringLength(3)]
        [RegularExpression(@"^[A-Za-z]{2,3}$", ErrorMessage = "Must be a 2 or 3 lettered Australian state")]
        public string? State { get; set; }
    }
}
