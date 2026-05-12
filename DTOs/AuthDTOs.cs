using System;
using System.ComponentModel.DataAnnotations;

namespace Compliance_Hub.api.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }

        public int? DepartmentId { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
        public DateTime ExpiryAt { get; set; }
    }
}
