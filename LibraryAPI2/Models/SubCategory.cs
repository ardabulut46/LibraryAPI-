using LibraryAPI2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI2.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string Name { get; set; } = "";

        public int CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public List<Book>? Books { get; set; }

    }
}
