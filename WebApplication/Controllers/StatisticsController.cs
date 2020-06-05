using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationRepository repository;

        public StatisticsController(ApplicationRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("length")]
        public async Task<double> OnGetLength([FromQuery]DateTime start, [FromQuery]DateTime end, int productId)
        {
            List<MetricsEntity> entities = await repository.getMetricsForTimePeriod(start, end, productId);

            return entities.Count;



        }

        [HttpGet("weekly")]
        public async Task<ActionResult<WeeklyStatistics>> OnGetWeekly([FromQuery]DateTime start, [FromQuery]DateTime end, [FromQuery] int productID)
        {

            if ((start != null && end != null && productID > 0) &&
                ((end - start).TotalDays >= 1 && (end - start).TotalDays <= 7))
            {

                //setting start date time to 00:00:00 in order to catch earliest metrics of the day
                //setting end date time to 23:59:59 in order to catch latest metrics of the day
                var startDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                var endDate = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

                WeeklyStatistics statistics = await GetWeeklyStatistics(startDate, endDate, productID);

                if (statistics != null)
                {
                    return statistics;
                }

                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }
        }

        /*[HttpGet("monthly")]

        public async Task<ActionResult<MonthlyStatistics>> OnGetMonthly([FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            if ((start != null && end != null) &&
                ((end - start).TotalDays >= 1 && (end - start).TotalDays <= 31))
            {
                MonthlyStatistics monthlyStatistics = new MonthlyStatistics { Metrics = new List<WeeklyMetrics>()};

                //setting start date time to 00:00:00 in order to catch earliest metrics of the day
                //setting end date time to 23:59:59 in order to catch latest metrics of the day
                var startDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                var endDate = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

                //setting calendar info to get week number of the year from DateTime object
                Calendar cal = new GregorianCalendar();
                CalendarWeekRule rule = CalendarWeekRule.FirstDay;
                DayOfWeek firstDay = DayOfWeek.Monday;

                //weeksDuration - for the loop to run 
                //week - number of the week to include in the Metrics of MonthlyStatistics
                //startOfWeek - 1st day of week in month
                //endOfWeek - last day of each week
                int weeksDuration = cal.GetWeekOfYear(endDate, rule, firstDay) - cal.GetWeekOfYear(startDate, rule, firstDay);
                int week = cal.GetWeekOfYear(startDate, rule, firstDay);
                DateTime startOfWeek = startDate;
                DateTime endOfWeek = new DateTime(start.Year, start.Month, start.Day, 23, 59, 59);
                //endOfWeek.AddDays(6);

                //temporary weekly statistics 
                WeeklyStatistics weeklyStatistics = null;

                for (int i=0;i<weeksDuration;i++)
                {
                    weeklyStatistics = await GetWeeklyStatistics(startOfWeek, endOfWeek);

                    if(weeklyStatistics!=null)
                    {
                        monthlyStatistics.Metrics.
                            Add(new WeeklyMetrics { WeekNo = week, Measurements = weeklyStatistics.Metrics });
                    }

                    else 
                    {
                        double[][] metrics = new double[7][];
                        for (int j = 0; j < metrics.Length; j++)
                        {
                            metrics[j] = new double[4];
                        }
                        monthlyStatistics.Metrics.
                             Add(new WeeklyMetrics { WeekNo = week, Measurements = metrics});
                    }
                    
                    week++;
                }

                monthlyStatistics.RoomID = weeklyStatistics.RoomID;
                monthlyStatistics.StartDate = startDate;
                monthlyStatistics.EndDate = endDate;
                monthlyStatistics.MonthNo = startDate.Month;
                monthlyStatistics.Year = startDate.Year;

                return monthlyStatistics;


            }
            else
            {
                return BadRequest();
            }
        }*/

        private async Task<WeeklyStatistics> GetWeeklyStatistics(DateTime start, DateTime end, int productID)
        {
            WeeklyStatistics statistics; //return final object
            double[][] metrics; //future metrics for weekly statistics
            DateTime currentDate; //hold current date
            int stepMetrics; //metricses in each step
            int currentStep; //current step
            int steps; //days specified in the week period

            //getting all metrics entities from db based on start and end dates
            List<MetricsEntity> entities = await repository.getMetricsForTimePeriod(start, end, productID);

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


    }
}