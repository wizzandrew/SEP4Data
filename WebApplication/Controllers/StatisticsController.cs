using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class StatisticsController : ControllerBase
    {
        public readonly IApplicationRepository repository;

        public StatisticsController(IApplicationRepository repository)
        {
            this.repository = repository;
        }

        ///<summary>
        ///Get weekly statistic specifying start and end dates, productId
        ///</summary>
        ///<param name="start">start date for statistic</param>
        ///<param name="end">end date for statistic</param>
        ///<param name="productID">Id of the product</param>
        ///<returns>A weekly statistics object or status code</returns>
        [ProducesResponseType(typeof(WeeklyStatistics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("weekly")]
        public async Task<ActionResult<WeeklyStatistics>> OnGetWeekly([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery] int productID)
        {
            WeeklyStatistics statistics;

            if ((start != null && end != null && productID > 0) &&
                ((end - start).TotalDays >= 1 && (end - start).TotalDays <= 7))
            {

                //setting start date time to 00:00:00 in order to catch earliest metrics of the day
                //setting end date time to 23:59:59 in order to catch latest metrics of the day
                var startDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                var endDate = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

                try
                {
                    statistics = await GetWeeklyStatistics(startDate, endDate, productID);

                    if (statistics != null)
                    {
                        return statistics;
                    }

                    else
                    {
                        return NotFound("No weekly statistics for such dates and productId");
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return StatusCode(500, "Internal server error");
                }

            }

            return BadRequest("Start/End date or ProductId not applicable");

        }

        /// <summary>
        /// Get weekly statistics object from database 
        /// </summary>
        /// <param name="start">start date for statistic</param>
        /// <param name="end">end date for statistic</param>
        /// <param name="productID">Id of the product</param>
        /// <returns>A weekly statistics object</returns>
        private async Task<WeeklyStatistics> GetWeeklyStatistics(DateTime start, DateTime end, int productID)
        {
            WeeklyStatistics statistics; //return final object
            List<MetricsEntity> entities; //metrics entities which will be taken from db
            double[][] metrics; //future metrics for weekly statistics
            DateTime currentDate; //holds current date
            int stepMetrics; //metricses in each step
            int currentStep; //current step
            int steps; //days specified in the week period

            try
            {
                //getting all metrics entities from db based on start and end dates
                entities = await repository.GetMetricsForTimePeriod(start, end, productID);

                if (entities.Count > 0)
                {
                    //initializing metrics as jagged array
                    metrics = new double[7][];
                    for (int i = 0; i < metrics.Length; i++)
                    {
                        metrics[i] = new double[4];
                    }

                    //date for comparison of current day and month of entities from db
                    //based on the same day metrics entities will be loaded to temp
                    currentDate = start;

                    //initializing steps
                    stepMetrics = 0;
                    currentStep = 0;
                    steps = (end - start).Days + 1;

                    while (currentStep < steps)
                    {
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i].LastUpdated.Value.Day == currentDate.Day &&
                                entities[i].LastUpdated.Value.Month == currentDate.Month)
                            {
                                //adding metrics values  
                                metrics[currentStep][0] += entities[i].Humidity;
                                metrics[currentStep][1] += entities[i].Temperature;
                                metrics[currentStep][2] += entities[i].Noise;
                                metrics[currentStep][3] += entities[i].CO2;

                                stepMetrics++;

                            }
                        }

                        //counting and rounding average metricsvalues
                        metrics[currentStep][0] = Math.Round((metrics[currentStep][0] / stepMetrics), 2);
                        metrics[currentStep][1] = Math.Round((metrics[currentStep][1] / stepMetrics), 2);
                        metrics[currentStep][2] = Math.Round((metrics[currentStep][2] / stepMetrics), 2);
                        metrics[currentStep][3] = Math.Round((metrics[currentStep][3] / stepMetrics), 2);

                        //setting step metrics back to 0
                        //incrementing currentStep
                        //incrementing currentDate
                        stepMetrics = 0;
                        currentStep++;
                        currentDate = currentDate.AddDays(1);

                    }

                    //giving statistics necessary values
                    statistics = new WeeklyStatistics { StartDate = start, EndDate = end, Year = start.Year };
                    statistics.ProductID = entities[0].ProductID;
                    statistics.Metrics = metrics;
                    statistics.WeekNo = new GregorianCalendar().
                        GetWeekOfYear(start, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    return statistics;
                }

                else
                {
                    return null;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }

        }

    }
}