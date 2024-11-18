using MeasurementPublisherAPI.Context;
using MeasurementPublisherAPI.Models;
using MeasurementPublisherAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeasurementPublisherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementController : ControllerBase
    {

        private readonly IMessageProducer _messageProducer;
        private readonly IMessageConsumer _messageConsumer;
        private readonly MeasurementContext _context;

        public MeasurementController(IMessageProducer messageProducer, MeasurementContext context, IMessageConsumer messageConsumer)
        {
            _messageProducer = messageProducer;
            _context = context;
            _messageConsumer = messageConsumer;
        }

        [HttpPost]
        public async Task<ActionResult<Measurement>> Post( Measurement measurement)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Measurements.Add(measurement);
            await _context.SaveChangesAsync();

            _messageProducer.SendMessage<Measurement>(measurement);

            return CreatedAtAction(nameof(GetMeasurement), new {id = measurement.Id}, measurement);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Measurement>> GetMeasurement(int id)
        {
            var measurement = await _context.Measurements.FindAsync(id);


            if (measurement == null)
            {
                return NotFound();
            }
            _messageConsumer.ReadMessage<Measurement>();
           

            return measurement;
        }

    }
}
