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
    public class RecommendedLevelsController : ControllerBase
    {
        private readonly ApplicationRepository repository;

        public RecommendedLevelsController(ApplicationRepository repository)
        {
            this.repository = repository;
        }

        // GET: api/RecommendedLevels
        [HttpGet]
        public async Task<ActionResult<RecommendedLevels>> onGet()
        {
            RecommendedLevelsEntity levelsEntity = await repository.GetRecommendedLevels();

            //returning RecommendedLevels given data from RecommendedLevelsEntity
            RecommendedLevels levels = new RecommendedLevels
            {
                Humidity = levelsEntity.Humidity,
                Temperature = levelsEntity.Temperature,
                Noise = levelsEntity.Noise,
                CO2 = levelsEntity.CO2
            };

            return Ok(levels);
        }
    }
}