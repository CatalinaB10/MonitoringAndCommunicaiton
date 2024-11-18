using MeasurementPublisherAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MeasurementPublisherAPI.Context
{
    public class MeasurementContext : DbContext
    {
        public MeasurementContext(DbContextOptions<MeasurementContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<Measurement> Measurements { get; set; } = default!;
    }
}
