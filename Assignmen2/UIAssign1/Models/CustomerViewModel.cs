using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(128)] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [StringLength(20)] public string PhoneNumber { get; set; }
        [StringLength(256)] public string Address { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
