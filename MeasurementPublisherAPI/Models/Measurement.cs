using System.ComponentModel.DataAnnotations;

namespace MeasurementPublisherAPI.Models
{
    public class Measurement
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public Guid DeviceId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
