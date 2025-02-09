using System.ComponentModel.DataAnnotations;

namespace LandOfDreamsAdminApp.Models
{
    public class Reservation 
    {
        public int Id { get; set; }
        public DateTime? BookingDate { get; set; } = DateTime.Now.Date;
        public DateTime? TravelEndDate { get; set; }
        public int? NumberOfPeople { get; set; }
        public double? TotalCost { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumberPrefix { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }

        public List<Booking>? Bookings { get; set; }
    }
}
