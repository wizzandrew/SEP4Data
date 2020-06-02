using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class WeeklyStatistics
    {
        public int RoomID { get; set; }
        public double[][] Metrics {get; set;}
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WeekNo { get; set; }
        public int Year { get; set; }
    }
}
