using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Booking
    {
        [Key] // Primary key for BookingId
        public int BookingId { get; set; }

        // Foreign keys for EventId and VenueId
        [ForeignKey("Event")]
        public int EventId { get; set; }


        [ForeignKey("Venue")]
        public int VenueId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        // Navigation properties
        public virtual Event? Event { get; set; }
        public virtual Venue? Venue { get; set; }
    }
}
