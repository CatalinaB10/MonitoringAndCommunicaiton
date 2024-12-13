using MeasurementPublisherAPI.Context;
using MeasurementPublisherAPI.Models;
using MeasurementPublisherAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeasurementPublisherAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class MeasurementController : ControllerBase
    { 
        private readonly MeasurementContext _context;

        public MeasurementController(MeasurementContext context )
        { 
            _context = context;
        }

        //[HttpGet("/measurementQueue")]
        //[EnableCors("AllowOrigin")]
        //public Task ReadAndStoreMeasurement()
        //{
        //    if (_messageConsumer.ReadMessage().IsCompleted)
        //    {
        //        return Task.FromResult(Ok());
        //    }
        //    return Task.FromResult(Ok(false));
        //}

        //[HttpGet("/deviceQueue")]
        //[EnableCors("AllowOrigin")]
        //public Task ReadAndStoreDevice()
        //{
        //    if (_messageConsumer.ReadDevice().IsCompleted)
        //    {
        //        return Task.FromResult(Ok());
        //    }
        //    return Task.FromResult(Ok(false));
        //}

        [HttpGet("/getAllMeasurements")]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetAllMasurements()
        {
            return await _context.Measurements.ToListAsync();
        }

        [HttpGet("/getAllDevices")]
     
        public async Task<ActionResult<IEnumerable<Device>>> GetAllDevices()
        {
            return await _context.Devices.ToListAsync();
        }

        [HttpGet("/countMeasurements")]
        public async Task<ActionResult<int>> CountMeasurements()
        {
            var nr = _context.Measurements.Count();
            return Ok(nr);
        }
        [HttpGet("/countDevicess")]
      
        public async Task<ActionResult<int>> GetCountDevices()
        {
            var nr = _context.Devices.Count();
            return Ok(nr);
        }

        [HttpDelete("/deleteDevices")]
        public async Task<IActionResult> DeleteDevices()
        {
            _context.Devices.RemoveRange(_context.Devices);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("/deleteMeasurements")]
       
        public async Task<IActionResult> DeleteMeasurements()
        {
            _context.Measurements.RemoveRange(_context.Measurements);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
