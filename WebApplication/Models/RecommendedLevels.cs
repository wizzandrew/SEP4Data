using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class RecommendedLevels
    {
        [Required]
        public double Humidity { get; set; }

        [Required]
        public double Temperature { get; set; }

        [Required]
        public double Noise { get; set; }

        [Required]
        public double CO2 { get; set; }
    }
}
