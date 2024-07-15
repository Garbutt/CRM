namespace CRM.Server.Models
{
    public class User
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? contactNumber { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public string? status { get; set; }
        
    }
}
