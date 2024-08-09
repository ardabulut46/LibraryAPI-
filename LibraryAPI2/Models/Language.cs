using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LibraryAPI2.Models;

namespace LibraryAPI2.Models
{
    public class Language
    {
        [Key]
        [Required]
        [StringLength(3, MinimumLength = 3)]
        [Column(TypeName = "char(13)")]
        public string Code { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = "";

        public List<Book>? Books { get; set; }
    }
}
