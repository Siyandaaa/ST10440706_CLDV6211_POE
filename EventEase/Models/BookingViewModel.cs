using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class BookingViewModel
    {
        public List<Booking>? Bookings { get; set; }
        [DataType(DataType.DateTime)]
        public SelectList? Dates { get; set; }
        public DateTime BookingDates { get; set; }
        public string? searchString { get; set; }
    }
}
