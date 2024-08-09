using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI2.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(800, MinimumLength = 1)]
        [Column(TypeName = "varchar(800)")]
        public string FullName { get; set; } = "";

        [Column(TypeName = "nvarchar(2000)")]
        public string? Biography { get; set; }

        [Range(-4000, 2100)]
        public short BirthYear { get; set; }

        [Range(-4000, 2100)]
        public short? DeathYear { get; set; }

        [JsonIgnore]
        public List<AuthorBook>? AuthorBooks { get; set; }
    }
}
