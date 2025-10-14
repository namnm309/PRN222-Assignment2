using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.ViewModels
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        [Required, StringLength(128)] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [StringLength(20)] public string PhoneNumber { get; set; } = string.Empty;
        [StringLength(256)] public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
