using Microsoft.CodeAnalysis.FlowAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace LibraryAPI2.Models
{
    
    public class BookRating
    {
        [Key]
        public int BookId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BookId))]    
        public Book? Book { get; set; }

        [JsonIgnore]
        public int RatingCount { get; set; }

        [JsonIgnore]
        public float  RatingSum { get; set; }

        public float Rating { get { return RatingSum / RatingCount; } set { } }
                 
    }
}
