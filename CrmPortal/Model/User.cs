using System;
using System.ComponentModel.DataAnnotations;

namespace CrmPortal.Model
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Password { get; set; }

        public bool IsActive { get; set; }
    }

    public class Token
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTimeOffset LoggedInDateTime { get; set; }

        public string IP { get; set; }
    }
}
