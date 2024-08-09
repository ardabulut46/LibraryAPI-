using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text.Json.Serialization;

namespace LibraryAPI2.Models
{
    public class Book
    {

        public int Id { get; set; }

        public int HowManyCopy {  get; set; }  

        [StringLength(13, MinimumLength = 10)]
        [Column(TypeName = "varchar(13)")]
        public string? ISBN { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        [Column(TypeName = "varchar(2000)")]
        public string Title { get; set; } = "";

        [Range(1, short.MaxValue)]
        public short PageCount { get; set; }

        [Range(-4000, 2100)]
        public short PublishingYear { get; set; }

        [StringLength(3000)]
        [Column(TypeName = "varchar(3000)")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int PrintCount { get; set; }


        public bool Banned { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; } 

        public int PublisherId { get; set; }
        [ForeignKey(nameof(PublisherId))]
        public Publisher? Publisher { get; set; }

        [StringLength(6, MinimumLength = 3)]
        [Column(TypeName = "varchar(6)")]
        public string LocationShelf { get; set; } = "";
        [ForeignKey(nameof(LocationShelf))]
        public Location? Location { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public int SubCategoryId { get; set; }
        [ForeignKey(nameof(SubCategoryId))]
        public SubCategory? SubCategory { get; set; }

        public string LanguageCode { get; set; } = "";
        [ForeignKey(nameof(LanguageCode))]
        public Language? Language { get; set; }

        [JsonIgnore]
        public List<AuthorBook>? AuthorBook { get; set; }  
        

    }
}
