using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class MonthlyStatistics
    {

        public int RoomID { get; set; }
        public List<WeeklyMetrics> Metrics { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MonthNo { get; set; }
        public int Year { get; set; }       

    }

    
}
