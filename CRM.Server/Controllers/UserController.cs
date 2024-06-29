
using CRM.Server.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;




namespace CRM.Server.Controllers
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly CMSDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserController> _logger;

        Response response = new Response();

        public UserController(CMSDbContext context, IWebHostEnvironment environment, ILogger<UserController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost, Route("signup")]
        public IActionResult Signup([FromBody] CRM.Server.Models.User user)
        {
            try
            {
                var userObj = _context.Users
                    .Where(u => u.email == user.email).FirstOrDefault();
                if (userObj == null)
                {
                    user.role = "user";
                    user.status = "false";
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return Ok(new { Message = "Successfully Registered" });
                }
                else
                {
                    return BadRequest(new { message = "Email already exists" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                User userObj = _context.Users
                    .Where(u => (u.email == loginRequest.email) && (u.password == loginRequest.password)).FirstOrDefault();
                if (userObj != null)
                {
                    switch (userObj.status) 
                    {
                        case "true":
                            return Ok(new
                            {
                                token = TokenManager.GenerateToken(userObj.email, userObj.role),
                                status = "true"
                            });
                        case "false":
                            return Ok(new
                            {
                                status = "false"
                            });
                        default:
                            return Ok(new { status = "unkown" });
                    }
                }
                else
                {
                    return Unauthorized(new { message = "Incorrect email or password" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet, Route("checkToken")]
        [CustomAuthenticationFilter]
        public IActionResult CheckToken()
        {
            return Ok( new { message = "Token is valid" });
        }

        [HttpGet, Route("getAllUsers")]
        [CustomAuthenticationFilter]
        public IActionResult GetAllUsers()
        {
            try
            {

                string? authorizationHeader = Request.Headers["authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    _logger.LogError("Headers are empty");
                    return Unauthorized(new { message = "Headers are empty" });
                }

             if(!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Token does not start with bearer");
                    return Unauthorized(new { message = "Token does not start with bearer" });
                }

             var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

                if(tokenClaim == null)
                {
                    _logger.LogError("Token is null here.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (tokenClaim.role != "admin")
                {
                    _logger.LogError("Admin role required to get all users.");
                    return Unauthorized(new { message = "Admin role required" });
                }

                var users = _context.Users.ToList();
                _logger.LogInformation($"Fetched {users.Count} users from database");

                if (!users.Any())
                {
                    return NotFound(new { message = "No users found" });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching users from database");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost, Route("updateUserStatus")]
        [CustomAuthenticationFilter]
        public IActionResult UpdateUserStatus(User user)
        {
            try
            {
                var token = Request.Headers["authorization"].First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);
                if (tokenClaim.role != "admin")
                {
                    return Unauthorized();
                }
                User userObj = _context.Users.Find(user.Id);
                if (userObj == null)
                {
                    response.Message = "User id is not found";
                    return BadRequest(Response);
                }
                userObj.status = user.status;
                _context.Entry(userObj).State = EntityState.Modified;
                _context.SaveChanges();
                response.Message = "User status updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost, Route("changePassword")]
        [CustomAuthenticationFilter]
        public IActionResult ChangePassword(ChangePassword changePassword)
        {
            try
            {
                string? authorizationHeader = Request.Headers["authorization"].First();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    _logger.LogError("Headers are empty");
                    return Unauthorized(new { message = "Headers are empty" });
                }

                if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Token does not start with bearer uc");
                    return Unauthorized(new { message = "Token does not start with bearer" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

                if (tokenClaim == null)
                {
                    _logger.LogError("Token is null here.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                _logger.LogInformation($"{tokenClaim.email}");


                var userEmail = tokenClaim.email;

                User userObj = _context.Users
                    .AsEnumerable()
                    .FirstOrDefault(x => x.email.Equals(userEmail, StringComparison.OrdinalIgnoreCase));

                _logger.LogInformation($"Email: {userEmail}, obj: {userObj}");

                if (userObj == null)
                {
                    _logger.LogError($"User not found with email: {userEmail}");
                    return NotFound(new { message = "User not found" });
                }

                if(string.IsNullOrEmpty(changePassword.OldPassword))
                {
                    _logger.LogError("Old password is empty");
                    response.Message = "Old password is empty";
                    return BadRequest(response);
                }

                if(string.IsNullOrEmpty(changePassword.NewPassword))
                {
                    _logger.LogError("New password is empty");
                    response.Message = "New password is empty";
                    return BadRequest(response);
                }

                if(changePassword.NewPassword != changePassword.OldPassword)
                {
                    _logger.LogError("Incorrect old password");
                    response.Message = "Incorrect old password.");
                    return BadRequest(response);
                }

                if (userObj != null)
                {
                    userObj.password = changePassword.NewPassword;
                    _context.Entry(userObj).State = EntityState.Modified;
                    _context.SaveChanges();
                    response.Message = "Password changed successfully";
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while changing password: {ex.Message}");
                return StatusCode(500, "An error occured while processing your request.");
            }
        }

        private string createEmailBody(string email, string password)
        {
            try
            {
                string filePath = Path.Combine(_environment.ContentRootPath, "Template", "forgot-password.html");
                using (StreamReader reader = new StreamReader(filePath)) ;
                string body = System.IO.File.ReadAllText(filePath);
               
                body = body.Replace("{email}", email);
                body.Replace("{password}", password);
                body.Replace("{frontendUrl}", "http://localhost:4200/");

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpPost, Route("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] User user)
        {
            User userObj = _context.Users
                .Where(x => x.email == user.email).FirstOrDefault();
            response.Message = "Password sent to your email successfully";
            if (userObj == null)
            {
                return Ok(response);
            }
            var message = new MailMessage();
            message.To.Add(new MailAddress(user.email));
            message.Subject = "Password for G&D Management System";
            message.Body = createEmailBody(user.email, userObj.password);
            message.IsBodyHtml = true;
            using (var Smtp = new SmtpClient())
            {
                await Smtp.SendMailAsync(message);
                await Task.FromResult(0);
            }
            return Ok(response);
        }
    }
}
