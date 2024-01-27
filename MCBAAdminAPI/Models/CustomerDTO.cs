using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAdminAPI.Models
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
        public string? PostCode { get; set; }

        [StringLength(11)]
        public string? TFN { get; set; }

        [StringLength(12)]
        [RegularExpression(@"^04\d{2}\s\d{3}\s\d{3}$", ErrorMessage = "Mobile must be of the format: 04XX XXX XXX")]
        public string? Mobile { get; set; }

        [StringLength(3)]
        public string? State { get; set; }

    }
}
