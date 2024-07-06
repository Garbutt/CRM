using CRM.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace CRM.Server.Controllers
{


    [Route("api/sites")]
    public class SitesController : ControllerBase
    {
        private readonly CMSDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UserController> _logger;

        public SitesController(CMSDbContext context, IWebHostEnvironment environment, ILogger<UserController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost, Route("addSite")]
        public async Task<IActionResult> AddSite([FromForm] SiteDTO siteDTO)
        {
            try
            {

                string? authorizationHeader = Request.Headers["authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    _logger.LogError("Headers are empty");
                    return Unauthorized(new { message = "Headers are empty" });
                }

                if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Token does not start with bearer");
                    return Unauthorized(new { message = "Token does not start with bearer" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

                if (tokenClaim == null)
                {
                    _logger.LogError("Token is null here.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (tokenClaim.role != "admin")
                {
                    _logger.LogError("Admin role required to add site.");
                    return Unauthorized(new { message = "Admin role required" });
                }

                if (siteDTO.PhotoFile != null && siteDTO.PhotoFile.Length > 0)
                {
                    var uploadsDirectory = _environment.WebRootPath != null ?
                                            Path.Combine(_environment.WebRootPath, "uploads") :
                                            Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(siteDTO.PhotoFile.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await siteDTO.PhotoFile.CopyToAsync(fileStream);
                    }
                    var relativePath = Path.Combine("uploads", fileName);

                    var site = new Sites
                    {
                        name = siteDTO.name,
                        address = siteDTO.address,
                        completion = siteDTO.completion,
                        PhotoPath = relativePath
                    };

                    var siteObj = _context.Sites
                        .FirstOrDefault(x => x.name == site.name && x.address == site.address);

                    if (siteObj == null)
                    {
                        site.completion = 0;
                        _context.Sites.Add(site);
                        await _context.SaveChangesAsync();
                        return Ok(new { message = "Site added successfully" });
                    }
                    else
                    {
                        return BadRequest(new { message = "Site already exists" });
                    }
                }
                else 
                { 
                   return BadRequest(new { message = "Image file is null" });
                }
            }
            catch(JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error deserializing the site object");
                return BadRequest(jsonEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddSite");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpGet, Route("getSite")]
        public async Task<IActionResult> GetSite()
        {
            try
            {
                var sites = _context.Sites.ToList();
                _logger.LogInformation($"Sites fetched successfully: {sites}");

                if (!sites.Any())
                {
                    return NotFound(new { message = "No sites found" });
                }

                sites.ForEach(site =>
                {
                    site.PhotoPath = adjustPhotoPath(site.PhotoPath);
                });

                return Ok(sites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sites");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private string adjustPhotoPath(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}";
            var adjustedPath = $"{baseUrl}/{path.Replace("\\", "/")}";

            return adjustedPath;
        }

        [HttpDelete, Route("deleteSite")]
        public async Task<IActionResult> DeleteSite([FromQuery] int id)
        {
            try
            {
                string? authorizationHeader = Request.Headers["authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    _logger.LogError("Headers are empty");
                    return Unauthorized(new { message = "Headers are empty" });
                }

                if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Token does not start with bearer");
                    return Unauthorized(new { message = "Token does not start with bearer" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

                if (tokenClaim == null)
                {
                    _logger.LogError("Token is null here.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (tokenClaim.role != "admin")
                {
                    _logger.LogError("Admin role required to add site.");
                    return Unauthorized(new { message = "Admin role required" });
                }

                var site = _context.Sites.FirstOrDefault(x => x.id == id);

                if (site == null)
                {
                    return NotFound(new { message = "Site not found" });
                }

                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Site deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting site");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut, Route("updateSite")]
        public async Task<IActionResult> UpdateSite([FromForm] SiteDTO siteDTO)
        {
            try
            {
                string? authorizationHeader = Request.Headers["authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    _logger.LogError("Headers are empty");
                    return Unauthorized(new { message = "Headers are empty" });
                }

                if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogError("Token does not start with bearer");
                    return Unauthorized(new { message = "Token does not start with bearer" });
                }

                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                TokenClaim tokenClaim = TokenManager.ValidateToken(token, _logger);

                if (tokenClaim == null)
                {
                    _logger.LogError("Token is null here.");
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (tokenClaim.role != "admin")
                {
                    _logger.LogError("Admin role required to add site.");
                    return Unauthorized(new { message = "Admin role required" });
                }

                var site = _context.Sites.FirstOrDefault(x => x.id == siteDTO.id);

                if (site == null)
                {
                    return NotFound(new { message = "Site not found" });
                }

                if (siteDTO.PhotoFile != null && siteDTO.PhotoFile.Length > 0)
                {
                    var uploadsDirectory = _environment.WebRootPath != null ?
                                            Path.Combine(_environment.WebRootPath, "uploads") :
                                            Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    if (!Directory.Exists(uploadsDirectory))
                    {
                        Directory.CreateDirectory(uploadsDirectory);
                    }

                    var fileName = Path.GetFileName(siteDTO.PhotoFile.FileName);
                    var filePath = Path.Combine(uploadsDirectory, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await siteDTO.PhotoFile.CopyToAsync(fileStream);
                    }
                    var relativePath = Path.Combine("uploads", fileName);

                    site.name = siteDTO.name;
                    site.address = siteDTO.address;
                    site.completion = siteDTO.completion;
                    site.PhotoPath = relativePath;

                    _context.Sites.Update(site);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Site updated successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Image file is null" });
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Error deserializing the site object");
                return BadRequest(jsonEx);
            }
        }
    }
}
