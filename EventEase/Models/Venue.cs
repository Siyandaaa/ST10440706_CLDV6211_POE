using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }
        [Display(Name = "Venue Name")]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? VenueName { get; set; }
        [StringLength(50)]
        [Required]
        public string? Location { get; set; }
        [Range(1, 500)]
        [Required]
        public int Capacity { get; set; }
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
