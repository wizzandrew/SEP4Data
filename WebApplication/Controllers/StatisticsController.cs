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
        public async Task<int> OnGetLength([FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            List<MetricsEntity> entities = await repository.getMetricsForTimePeriod(start, end);

            return entities.Count;
        }

        [HttpGet]
        public async Task<ActionResult<WeeklyStatistics>> OnGetWeekly([FromQuery]DateTime start, [FromQuery]DateTime end)
        {

            if ((start != null && end != null) &&
                ((end - start).TotalDays >= 1 && (end - start).TotalDays <= 7))
            {

                //setting start date time to 00:00:00 in order to catch earliest metrics of the day
                //setting end date time to 23:59:59 in order to catch latest metrics of the day
                var startDate = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                var endDate = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);

                WeeklyStatistics statistics =  await GetWeeklyStatistics(startDate, endDate);

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

        [HttpGet("monthly")]

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
        }

        private async Task<WeeklyStatistics> GetWeeklyStatistics(DateTime start, DateTime end)
        {
            WeeklyStatistics statistics;

            //getting all metrics entities from db based on start and end dates
            List<MetricsEntity> entities = await repository.getMetricsForTimePeriod(start, end);

            if(entities.Count > 0)
            {
                //future metrics for weekly statistics
                double[][] metrics = new double[7][];

                //initializing metrics as jagged array
                for (int i = 0; i < metrics.Length; i++)
                {
                    metrics[i] = new double[4];
                }

                //temp list for holding measurements of one day
                List<Metrics> temp = new List<Metrics>();

                //day for comparison of current day and day of entities from db
                //based on the same day metrics entities will be loaded to temp
                int day = entities[0].LastUpdated.Value.Day;

                for (int i = 0; i < entities.Count; i++)
                {
                    if (entities[i].LastUpdated.Value.Day == day)
                    {
                        temp.Add(Metrics.getMetricsFromEntity(entities[i]));

                        if (i == entities.Count - 1 ||
                            entities[i].LastUpdated.Value.Day != entities[i + 1].LastUpdated.Value.Day)
                        {

                            //day of week Monday/Tuesday.....
                            string dayOfWeek = entities[i].LastUpdated.Value.DayOfWeek.ToString();

                            //index of the day of the week in metrics Monday-0, Tuesday-1, ...
                            int dayIndex = 0;
                            switch (dayOfWeek)
                            {
                                case "Monday":
                                    dayIndex = 0;
                                    break;
                                case "Tuesday":
                                    dayIndex = 1;
                                    break;
                                case "Wednesday":
                                    dayIndex = 2;
                                    break;
                                case "Thursday":
                                    dayIndex = 3;
                                    break;
                                case "Friday":
                                    dayIndex = 4;
                                    break;
                                case "Saturday":
                                    dayIndex = 5;
                                    break;
                                case "Sunday":
                                    dayIndex = 6;
                                    break;
                            }

                            //adding all measurements for the day
                            for (int j = 0; j < temp.Count; j++)
                            {

                                metrics[(dayIndex)][0] += temp[j].Humidity;
                                metrics[(dayIndex)][1] += temp[j].Temperature;
                                metrics[(dayIndex)][2] += temp[j].Noise;
                                metrics[(dayIndex)][3] += temp[j].CO2;

                            }

                            //counting averages of metrics from temp and loading to actual output
                            for (int k = 0; k < 4; k++)
                            {
                                metrics[(dayIndex)][k] /= temp.Count;
                            }

                            temp.Clear();

                            if (i < entities.Count - 1)
                            {
                                //changing the day
                                day = entities[i + 1].LastUpdated.Value.Day;
                            }


                        }
                    }

                    else if (entities[i].LastUpdated.Value.Day != day)
                    {
                        //changing the day
                        day = entities[i + 1].LastUpdated.Value.Day;
                    }
                }

                statistics = new WeeklyStatistics { StartDate = start, EndDate = end, Year = start.Year };
                statistics.RoomID = entities[0].R_ID;
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