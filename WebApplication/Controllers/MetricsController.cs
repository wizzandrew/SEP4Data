using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Data.Entities;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly ApplicationRepository repository;

        public MetricsController(ApplicationRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<Metrics>> onGet()
        {
            MetricsEntity me = await repository.getLastUpdatedMetrics();

            //returning Metrics given data from MetricsEntity
            if (me != null) { 
                Metrics metrics = new Metrics
                {
                MetricsID = me.MetricsID,
                Humidity = me.Humidity,
                Temperature = me.Temperature,
                Noise = me.Noise,
                CO2 = me.CO2,
                };

                if (me.LastUpdated.HasValue)
                {
                    metrics.LastUpdated = (DateTime)me.LastUpdated;
                }

                return Ok(metrics);
            }
            else
            {
                return NotFound();
            }
            


        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Metrics>> onGet(int id)
        {
            MetricsEntity me = await repository.getMetricsById(id);

            //returning Metrics given data from MetricsEntity
            if (me != null)
            {
                Metrics metrics = new Metrics
                {
                    MetricsID = me.MetricsID,
                    Humidity = me.Humidity,
                    Temperature = me.Temperature,
                    Noise = me.Noise,
                    CO2 = me.CO2,
                };

                if (me.LastUpdated.HasValue)
                {
                    metrics.LastUpdated = (DateTime)me.LastUpdated;
                }

                return Ok(metrics);
            }
            else
            {
                return NotFound();
            }
        }
    }
}