using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class WeeklyMetrics
    {
            public int WeekNo { get; set; }
            public double[][] Measurements { get; set; }
    }
}
