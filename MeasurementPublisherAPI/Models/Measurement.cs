using System.ComponentModel.DataAnnotations;

namespace MeasurementPublisherAPI.Models
{
    public class Measurement
    {
        [Key]
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public Guid? DeviceId { get; set; }
       
    }
}
