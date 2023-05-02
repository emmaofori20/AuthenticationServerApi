using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.DTOs
{
	public class LoginDTO
	{
		[Required(ErrorMessage = "User Name is required")]
		public string? Username { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string? Password { get; set; }
	}

	public class LoginUserDTO
	{
		public string Username { get; set; }
		public DateTime expirationDate { get; set; }
		public string Token { get; set; }
		public string Id { get; set; }
		public List<string> roles { get; set; }
	}

	public class AuthenticateModel
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public int ApplicationId { get; set; }
	}

    public class PasswordResetEmail
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
        public string Password { get; set; }
		
		[DataType(DataType.Password)]
		[Display(Name = "Confrim password")]
		[Compare("Password", ErrorMessage ="Password and Confrim Password must watch")]
        public string ConfrimPassword { get; set; }

        public string Token { get; set; }
    }
}
