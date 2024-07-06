namespace CRM.Server.Models
{
    public class SiteDTO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public int completion { get; set; }
        public IFormFile PhotoFile { get; set; }
    }
}
