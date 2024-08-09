using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LibraryAPI2.Models;
using System.Text.Json.Serialization;

namespace LibraryAPI2.Models
{
    public class AppUser : IdentityUser
    {

        public long IdNumber { get; set; }

        [StringLength(100, MinimumLength = 1)]
        [Column(TypeName = "varchar(100)")]
        public string FullName { get; set; } = "";


        public bool Gender { get; set; }

        [Required]
        [Range(-4000, 2100)]
        public int BirthDate { get; set; }


        [JsonIgnore]
        public DateTime RegisterDate { get; set; } = DateTime.Now;


        [NotMapped]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";

        [NotMapped]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = "";

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public  DateTime? DeletedDate {  get; set; }     

    }
    public class Member
    {
        [Key]
        public string Id { get; set; } = "";

        

        [ForeignKey(nameof(Id))]
        public AppUser? AppUser { get; set; }


    }
    public class Employee
    {
        [Key]
        public string Id { get; set; } = "";

        [ForeignKey(nameof(Id))]
        public AppUser? AppUser { get; set; }

        [Required]
        [Column(TypeName = "varchar(60)")]
        [StringLength(60, MinimumLength = 6)]
        public string Title { get; set; } = "";

        [Required]
        [Range(0, 100000)]
        public float Salary { get; set; }

        public bool Insurance { get; set; }


        public string? Shift { get; set; }


    }
   
}

