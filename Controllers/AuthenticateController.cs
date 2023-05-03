using Azure;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using shared.DTOs;
using shared.Entities;
using shared.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Net;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace AuthenticationServerApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticateController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly AuthenticationServerDBContext _context;
		private readonly IConfiguration _configuration;

		public AuthenticateController(
			UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			AuthenticationServerDBContext context,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
			_configuration = configuration;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO model)
		{
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var token = GetToken(authClaims);

				return Ok(new LoginUserDTO
				{
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					expirationDate = token.ValidTo,
					roles = userRoles.ToList(),
					Id = user.Id,
					Username = user.UserName,
				});
			}
			return Unauthorized();
		}


		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDTO model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Username);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User already exists!" });

			IdentityUser user = new()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Username
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO { Status = "Error", Message = "User creation failed! Please check user details and try again." });

			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			await _userManager.AddToRoleAsync(user, model.Role);

			return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully!" });
		}


		[HttpGet("Applications")]
		public async Task<ActionResult<IEnumerable<Application>>> GetApplication()
		{
			return await _context.Applications.ToListAsync();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
		{
			var user = await _userManager.Users.ToListAsync();
			List<UserDTO> users = new List<UserDTO>();
			foreach (var item in user)
			{
				var userRoles = await _userManager.GetRolesAsync(item);

				users.Add(new UserDTO
				{
					Email = item.Email,
					UserId = item.Id,
					username = item.UserName,
					roles = userRoles.ToList()
				});
			}
			return users;
		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<UserDTO>> GetUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user != null)
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				return new UserDTO
				{
					Email = user.Email,
					UserId = user.Id,
					username = user.UserName,
					roles = userRoles.ToList()

				};
			}

			return NotFound();
		}

		[Authorize]
		[HttpGet("GetUserApplicationAssignment/{id}")]
		public async Task<ActionResult<IEnumerable<UserApplication>>> GetUserApplicationAssignment(string id)
		{
			return await _context.UserApplications.Where(x => x.UserId == id).ToListAsync();
		}

        [HttpPost("SendPasswordResetLink")]
        public async Task<IActionResult> SendPasswordResetLink(PasswordResetEmail model)
        {
			try
			{
                string token = null;

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(user);
                }

                var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);
                var url = $"https://localhost:7253/Account/EnterPassword?email={model.Email}&token={token}"; // generate the URL for the password reset link
                                                                                                              ///////////////////////////////////////////////////////////
                var fromAddress = new MailAddress("kwadubanana@gmail.com", "Emmanuel Ofori");
                var toAddress = new MailAddress(model.Email);
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("kwadubanana@gmail.com", "zafnzqmqlrclmelr")
                };

				using (var message = new MailMessage(fromAddress, toAddress)
				{
					Subject = "Password Reset Request",
					Body = $"<html><p>Click the following link to reset your password: {url}</p></html>",
					IsBodyHtml=true,
                })
                {
                    smtpClient.Send(message);
                }


                return Ok();
            }
			catch (Exception ex)
			{

				throw;
			}
			

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
				if (result.Succeeded)
				{
					return Ok();
				}
                return BadRequest();
            }
            return BadRequest();
        }


        [HttpPost("Applications-Authenticator")]
		public async Task<IActionResult> ApplicationAuthenticator([FromBody] AuthenticateModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var token = GetToken(authClaims);
				bool check = CheckIfUserBelongsToApplication(user.Id, model.ApplicationId);
				return Ok(new 
				{
					Token = new JwtSecurityTokenHandler().WriteToken(token),
					Id = user.Id,
					Username = user.UserName,
					UserBelongsToApplication = check
				});
			}

			return Unauthorized();
		}

		[Authorize]
		[HttpPost("AssignUserToApplication")]
		public async Task<ActionResult> AssignUser([FromBody] UserAssignmentViewModel model)
		{
			if (model.userAssigned.Count > 0)
			{
				foreach (var _userAssigned in model.userAssigned)
				{
					if(! _userAssigned.IsAssigned) 
					{ 
						var removeitem = _context.UserApplications.Where(x=>x.ApplicationId == _userAssigned.ApplicationId && x.UserId == model.UserId).FirstOrDefault();
						if (removeitem != null)
						{
                            _context.UserApplications.Remove(removeitem);

                        }
                    }
					else
					{
                        var Additem = _context.UserApplications.Where(x => x.ApplicationId == _userAssigned.ApplicationId && x.UserId == model.UserId).FirstOrDefault();
						if(Additem == null)
						{
                            _context.UserApplications.Add(new UserApplication
                            {
                                ApplicationId = _userAssigned.ApplicationId,
                                UserId = model.UserId,
                                UserCredentials = model.username+ _userAssigned.Name.Split(' ')[0],
                            });
                        }
						else
						{

						}
                        
                    }
                   
                }
				
			}

			await _context.SaveChangesAsync();

			return Ok();
		}

		[Authorize]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			var rolesForUser = await _userManager.GetRolesAsync(user);
			if (rolesForUser.Count() > 0)
			{
				foreach (var item in rolesForUser.ToList())
				{
					// item should be the name of the role
					var result = await _userManager.RemoveFromRoleAsync(user, item);
				}
			}

			await _userManager.DeleteAsync(user);

			return NoContent();
		}

		private JwtSecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
				);

			return token;
		}
		private bool CheckIfUserBelongsToApplication(string UserId, int ApplicationId)
		{
			return _context.UserApplications.Any(x => x.UserId == UserId && x.ApplicationId == ApplicationId);
		}

	}
}
