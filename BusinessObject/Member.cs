using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }

        [Display(Name = "Member ID")]
        public int MemberId { get; set; }

        [Display(Name = "Member Name")]
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Fullname { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Company Name")]
        [StringLength(100, MinimumLength = 5)]
        public string CompanyName { get; set; }

        [Display(Name = "City")]
        [Required]
        public string City { get; set; }

        [Display(Name = "Country")]
        [Required]
        public string Country { get; set; }

        [Display(Name = "Password")]
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }
        

        public virtual ICollection<Order> Orders { get; set; }
    }
}
