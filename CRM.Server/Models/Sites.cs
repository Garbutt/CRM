using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CRM.Server.Models
{
    public class Sites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("address")]
        public string address { get; set; }
        public int completion { get; set; }
        [JsonPropertyName("photoPath")]
        public string? PhotoPath { get; set; }
    }
}
