using System.ComponentModel.DataAnnotations;

namespace MeasurementPublisherAPI.Models
{
    public class MeasurementDTO
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
