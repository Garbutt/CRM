
using CRM.Server.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;




namespace CRM.Server.Controllers
{
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly CMSDbContext _context;
        private readonly IWebHostEnvironment _environment;

        Response response = new Response();

        public UserController(CMSDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                User userObj = _context.Users
                    .Where(u => (u.email == user.email) && (u.password == user.password)).FirstOrDefault();
                if (userObj != null)
                {
                    if (userObj.status == "true")
                    {
                        return Ok(new { token = TokenManager.GenerateToken(userObj.email, userObj.role) });
                    }
                    else
                    {
                        return Unauthorized(new { message = "Wait for Admin approval" });
                    }
                }
                else
                {
                    return Unauthorized( new { message = "Incorrect email or password" });
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
                var token = Request.Headers["authorization"].First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
                if (tokenClaim.role != "admin")
                {
                    return Unauthorized();
                }
                var result = _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.name,
                        u.contactNumber,
                        u.email,
                        u.status,
                        u.role
                    })
                .Where(x => (x.role == "user"))
                .ToList();
                return Ok( result);
            }
            catch (Exception ex)
            {
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
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);
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
                var token = Request.Headers["authorization"].First();
                TokenClaim tokenClaim = TokenManager.ValidateToken(token);

                User userObj = _context.Users
                    .Where(x => (x.email == tokenClaim.email && x.password == changePassword.OldPassword)).FirstOrDefault();
                if (userObj != null)
                {
                    userObj.password = changePassword.NewPassword;
                    _context.Entry(userObj).State = EntityState.Modified;
                    _context.SaveChanges();
                    response.Message = "Password changed successfully";
                    return Ok(response);
                }
                else
                {
                    response.Message = "Incorrect old password";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
