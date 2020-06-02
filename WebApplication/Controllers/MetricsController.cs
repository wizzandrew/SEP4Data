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

            Metrics metrics;

            if (me != null)
            {

                metrics = Metrics.getMetricsFromEntity(me);
                return Ok(metrics);
            }
            else
            {
                return NotFound();
            }



        }


    }
}