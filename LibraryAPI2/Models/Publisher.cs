using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LibraryAPI2.Models;

namespace LibraryAPI2.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required]
        [StringLength(800)]
        [Column(TypeName = "varchar(800)")]
        public string Name { get; set; } = "";

        [Phone]
        [StringLength(15, MinimumLength = 7)]
        [Column(TypeName = "varchar(15)")]
        public string? Phone { get; set; } = "";

        [EmailAddress]
        [StringLength(320, MinimumLength = 5)]
        public string? Email { get; set; }

        [StringLength(800)]
        [Column(TypeName = "varchar(800)")]
        public string? ContactPerson { get; set; }

        [JsonIgnore]
        public List<Book>? Books { get; set; }
    }
}
