using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmPortal.Model
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }

        public bool IsActive { get; set; }

        [StringLength(10, MinimumLength = 10)]
        public string NationalCode { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public User CreatedBy { get; set; }

        public Guid CreatedById { get; set; }
    }
}
